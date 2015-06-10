/* RoaminSMPP: SMPP communication library
 * Copyright (C) 2004, 2005 Christopher M. Bouzek
 *
 * This file is part of RoaminSMPP.
 *
 * RoaminSMPP is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 *
 * RoaminSMPP is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with RoaminSMPP.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Net.Sockets;

namespace RoaminSMPP
{
	/// <summary>
	/// ����� ����������� ������ � ��������
	/// </summary>
	public class AsyncSocketClient : IDisposable
	{
		#region delegates

        /// <summary>
        /// ����������� ��� ��������� ���������
        /// </summary>
        public event MessageHandler MessageRecieved;

        /// <summary>
        /// ����������� ��� ���������� ������
        /// </summary>
        public event SocketClosingHandler SocketClosed;

        /// <summary>
        /// ����������� ��� ������ ������ ��� ��������
        /// </summary>
        public event ErrorHandler TranciverFailed;
        
        /// <summary>
		/// ����������, ����� ��������� ����� ���������.
		/// </summary>
		/// <param name="asyncSocketClient">
		/// ������ � �������� �������.
		/// </param>
		public delegate void MessageHandler(AsyncSocketClient asyncSocketClient);
	
		/// <summary>
		/// ����������, ����� ���������� ����������.
		/// </summary>
		/// <param name="asyncSocketClient">
		/// ������ � �������� �������.
		/// </param>
		public delegate void SocketClosingHandler(
			AsyncSocketClient asyncSocketClient);
	
		/// <summary>
		/// ���������� ����� ������ �� �����
		/// </summary>
		/// <param name="asyncSocketClient">
		/// The AsyncSocketClient to receive messages from.
		/// </param>
		/// <param name="exception">
		/// ����������, ������� �������� ������.
		/// </param>
		public delegate void ErrorHandler(
			AsyncSocketClient asyncSocketClient, Exception exception);
	
		#endregion delegates
		//���� ��������
		private const int BUFFER_SIZE = 1048576;

		#region private fields

        private NetworkStream _NetworkStream;
		private TcpClient _TcpClient;
		private AsyncCallback _CallbackReadMethod;
		private AsyncCallback _CallbackWriteMethod;
		private MessageHandler _MessageHandler;
		private SocketClosingHandler _SocketCloseHandler;
		private ErrorHandler _ErrorHandler;
		private bool _IsDisposed;
		private string _ServerIPAddress;
		private Int16 _ServerPort;
		private object _StateObject;
		private byte[] _Buffer;

		#endregion private fields

		#region properties

		/// <summary>
		/// ����� �������� �������
		/// </summary>
		public string ServerIPAddress
		{
			get
			{
				return _ServerIPAddress;
			}
		}

		/// <summary>
		/// ���� �������� �������
		/// </summary>
		public Int16 ServerPort
		{
			get
			{
				return _ServerPort;
			}
		}

		/// <summary>
		/// ������ ����������������� ���������, ������������ � �����������.
		/// </summary>
		public object StateObject
		{
			get
			{
				return _StateObject;
			}
			set
			{
				_StateObject = value;
			}
		}

		/// <summary>
		/// ����� ������/������
		/// </summary>
		public byte[] Buffer
		{
			get
			{
				return _Buffer;
			}
		}

		#endregion properties

        /// <summary>
        /// ����������� AsyncSocketClient.
        /// </summary>
        /// <param name="client">������� ������</param>
        /// <param name="bufferSize">������ ������ ������.</param>
        /// <param name="stateObject">������, ������������ ��� �������� ���������� � ��������� </param>
        public AsyncSocketClient(TcpClient client, Int32 bufferSize, object stateObject)
            : this(bufferSize, stateObject, null, null, null)
        {
            _TcpClient = client;
            InitTcpClient();
        }
        
        
        /// <summary>
		/// ����������� AsyncSocketClient.
		/// </summary>
        /// <param name="client">������� ������</param>
		/// <param name="bufferSize">������ ������ ������.</param>
        /// <param name="stateObject">������, ������������ ��� �������� ���������� � ��������� </param>
		/// <param name="msgHandler">����� ������ ���������.</param>
		/// <param name="closingHandler">����� ��������� �������� �����������.</param>
		/// <param name="errHandler">����� ��������� ��������� ��������.</param>
        public AsyncSocketClient(TcpClient client, Int32 bufferSize, object stateObject,
                                 MessageHandler msgHandler, SocketClosingHandler closingHandler,
                                 ErrorHandler errHandler): this(bufferSize, stateObject, msgHandler, closingHandler, errHandler)
        {
            _TcpClient = client;
            InitTcpClient();
        }
        
        /// <summary>
		/// ����������� AsyncSocketClient.
		/// </summary>
		/// <param name="bufferSize">������ ������ ������.</param>
        /// <param name="stateObject">������, ������������ ��� �������� ���������� � ��������� </param>
		/// <param name="msgHandler">����� ������ ���������.</param>
		/// <param name="closingHandler">����� ��������� �������� �����������.</param>
		/// <param name="errHandler">����� ��������� ��������� ��������.</param>
		public AsyncSocketClient(Int32 bufferSize, object stateObject,
		                         MessageHandler msgHandler, SocketClosingHandler closingHandler,
		                         ErrorHandler errHandler)
		{
			_Buffer = bufferSize == 0? new byte[BUFFER_SIZE]: new byte[bufferSize];
			_StateObject = stateObject;

			//����������� ������������
            if (msgHandler != null)
                MessageRecieved += msgHandler;
			if (closingHandler!=null)
			    SocketClosed += closingHandler;
            if (errHandler!= null)
			    TranciverFailed += errHandler;

			//����������� ������������ ����������� �������
			_CallbackReadMethod = new AsyncCallback(ReceiveComplete);
			_CallbackWriteMethod = new AsyncCallback(SendComplete);

			//�� ��� �� ����������
			_IsDisposed = false;
		}

		/// <summary>
		/// ����� �����������.  ���� ����� Dispose() ��� ��������� ������, �� ��� ������ ������
		/// </summary>
		~AsyncSocketClient()
		{
			if (!_IsDisposed)
			{
				Dispose();
			}
		}

		#region public methods

		/// <summary>
		/// ���������� ���� ����������� � ����������� �����
		/// </summary>
		public void Dispose()
		{
			try
			{
				_IsDisposed = true;
				Disconnect();
			}
			catch
			{
                //������� ��� ���������� ������ �� �������������
            }
		}

		/// <summary>
		/// �������������� � ������� �� ��������� ������
		/// ����� �������� Receive().
		/// </summary>
		/// <param name="IPAddress">����� �������.</param>
		/// <param name="port">���� �������.</param>
		public void Connect(String IPAddress, Int16 port)
		{
			try
			{
				//���������� ������� ��� �� �������?
				if (_NetworkStream == null)
				{
					_ServerIPAddress = IPAddress;
					_ServerPort = port;

					//�������� �������� ����������
					_TcpClient = new TcpClient(_ServerIPAddress, _ServerPort);
                    InitTcpClient();

					//�������� ����� ���������
					Receive();
				}
			}
			catch (SocketException exc)
			{
				//���������� ���������� �� �������
				throw new Exception(exc.Message, exc.InnerException);
			}
		}

        /// <summary>
        /// ������������� �������
        /// </summary>
        private void InitTcpClient()
        {
            _NetworkStream = _TcpClient.GetStream();
            //��������� ��������� ������
            _TcpClient.ReceiveBufferSize = BUFFER_SIZE;
            _TcpClient.SendBufferSize = BUFFER_SIZE;
            _TcpClient.NoDelay = true;
            //���� ���������� ������, ������� ��� ��������������� ������
            _TcpClient.LingerState = new LingerOption(false, 0);
        }

		///<summary>
		/// ������������
		/// </summary>
		public void Disconnect()
		{
			//��������� ���������� � ��������������� ��������� ��� �������������
			if (_NetworkStream != null)
			{
				_NetworkStream.Close();
			}
			if (_TcpClient != null)
			{
				_TcpClient.Close();
			}

			//���������� ��� �������� ������, �� �������� ������� ��� ��� ������������ ��� ����������
			_NetworkStream = null;
			_TcpClient = null;
            //���������� �������, ���� �� ���� ��������
            OnSocketClose();
		}

        protected void OnSocketClose()
        {
            if (SocketClosed != null)
                SocketClosed(this);
        }

		///<summary>
		/// ���������� �������� ������
		/// </summary>
		/// <param name="buffer">
		/// ����� ������ ��� ��������
		/// </param>
		public void Send(byte[] buffer)
		{
			//�������� ������; �� ���������� � ��������� �����-���� ���������� � ���������
			try
			{
				if (_NetworkStream != null && _NetworkStream.CanWrite)
				{
					_NetworkStream.BeginWrite(
						buffer, 0, buffer.Length, _CallbackWriteMethod, null);
				}
				else
				{
					throw new Exception("Socket is closed, cannot Send().");
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// ����������� ����� ������
		/// </summary>
		public void Receive()
		{
			try
			{
				if (_NetworkStream != null && _NetworkStream.CanRead)
				{
					_Buffer = new byte[BUFFER_SIZE];
					
					_NetworkStream.BeginRead(
						_Buffer, 0, _Buffer.Length, _CallbackReadMethod, null);
				}
				else
				{
					throw new Exception("Socket is closed, cannot Receive().");
				}
			}
			catch
			{
				throw;
			}
		}

		#endregion public methods

		#region private methods

		/// <summary>
		/// ���������� ��� ������� ���������
		/// </summary>
		/// <param name="state">������ ���������, ���������� ���������� � ����������.</param>
		private void SendComplete(IAsyncResult state)
		{
			try
			{
				//��������� ��� ����� ������� ��� ������
				if (_NetworkStream.CanWrite)
				{
					_NetworkStream.EndWrite(state);
				}
			}
			catch(Exception ex)
			{
                //�� ����������� - �����, ���� �������� ���������
                try
                {
                    OnError(ex);
                }
                finally
                {
                    Dispose();
                }
            }
		}

        protected void OnError(Exception ex)
        {
            if (TranciverFailed != null)
                TranciverFailed(this, ex);
        }

		/// <summary>
		/// ���������� ��� ��������� ���������
		/// </summary>
        /// <param name="state">������ ���������, ���������� ���������� � ����������.</param>
		private void ReceiveComplete(IAsyncResult state)
		{
			try
			{
				//���������, ��� ����� ������� ��� ������
				if (_NetworkStream.CanRead)
				{
					int bytesReceived = _NetworkStream.EndRead(state);

					//���� ���� ���������� ������, �� ��� �����, ����� ����� ���������
                    // � ���� ����������
					if (bytesReceived > 0)
					{
						try
						{
							//��������� ���������� ��������� �����������
                            OnMessageRecieved();
						}
						finally
						{
							//������� ���������� ����������
							Receive();
						}
					}
				}
			}
			catch (Exception ex)
			{
                //������ �����, �������� �������� ��� ������	
                try
                {
                    if (_ErrorHandler!=null)
                        _ErrorHandler(this, ex);
                }
                finally
                {
                    Dispose();
                }
			}
		}

        protected void OnMessageRecieved()
        {
            if (MessageRecieved != null)
                MessageRecieved(this);
        }
		#endregion private methods
	}
}
