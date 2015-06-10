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
	/// Класс асинхронной работы с сокетами
	/// </summary>
	public class AsyncSocketClient : IDisposable
	{
		#region delegates

        /// <summary>
        /// Срабатывает при получении сообщения
        /// </summary>
        public event MessageHandler MessageRecieved;

        /// <summary>
        /// Срабатывает при отключении сокета
        /// </summary>
        public event SocketClosingHandler SocketClosed;

        /// <summary>
        /// Срабатывает при ошибке приема или передачи
        /// </summary>
        public event ErrorHandler TranciverFailed;
        
        /// <summary>
		/// Вызывается, когда поступает новое сообщение.
		/// </summary>
		/// <param name="asyncSocketClient">
		/// Клиент с которого событие.
		/// </param>
		public delegate void MessageHandler(AsyncSocketClient asyncSocketClient);
	
		/// <summary>
		/// Вызывается, когда обрывается соединение.
		/// </summary>
		/// <param name="asyncSocketClient">
		/// Клиент с которого событие.
		/// </param>
		public delegate void SocketClosingHandler(
			AsyncSocketClient asyncSocketClient);
	
		/// <summary>
		/// Вызывается когда ошибка на линии
		/// </summary>
		/// <param name="asyncSocketClient">
		/// The AsyncSocketClient to receive messages from.
		/// </param>
		/// <param name="exception">
		/// Исключение, которое породило ошибку.
		/// </param>
		public delegate void ErrorHandler(
			AsyncSocketClient asyncSocketClient, Exception exception);
	
		#endregion delegates
		//Один мегабайт
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
		/// Адрес ответной стороны
		/// </summary>
		public string ServerIPAddress
		{
			get
			{
				return _ServerIPAddress;
			}
		}

		/// <summary>
		/// Порт ответной стороны
		/// </summary>
		public Int16 ServerPort
		{
			get
			{
				return _ServerPort;
			}
		}

		/// <summary>
		/// Объект пользовательского состояния, ассоцируемый с соединением.
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
		/// Буфер чтения/записи
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
        /// Конструктор AsyncSocketClient.
        /// </summary>
        /// <param name="client">Готовый клиент</param>
        /// <param name="bufferSize">Размер буфера приема.</param>
        /// <param name="stateObject">Объект, используемый для отправки информации о состоянии </param>
        public AsyncSocketClient(TcpClient client, Int32 bufferSize, object stateObject)
            : this(bufferSize, stateObject, null, null, null)
        {
            _TcpClient = client;
            InitTcpClient();
        }
        
        
        /// <summary>
		/// Конструктор AsyncSocketClient.
		/// </summary>
        /// <param name="client">Готовый клиент</param>
		/// <param name="bufferSize">Размер буфера приема.</param>
        /// <param name="stateObject">Объект, используемый для отправки информации о состоянии </param>
		/// <param name="msgHandler">Метод приема сообщений.</param>
		/// <param name="closingHandler">Метод обработки закрытия подключения.</param>
		/// <param name="errHandler">Метод обработки ошибочных ситуаций.</param>
        public AsyncSocketClient(TcpClient client, Int32 bufferSize, object stateObject,
                                 MessageHandler msgHandler, SocketClosingHandler closingHandler,
                                 ErrorHandler errHandler): this(bufferSize, stateObject, msgHandler, closingHandler, errHandler)
        {
            _TcpClient = client;
            InitTcpClient();
        }
        
        /// <summary>
		/// Конструктор AsyncSocketClient.
		/// </summary>
		/// <param name="bufferSize">Размер буфера приема.</param>
        /// <param name="stateObject">Объект, используемый для отправки информации о состоянии </param>
		/// <param name="msgHandler">Метод приема сообщений.</param>
		/// <param name="closingHandler">Метод обработки закрытия подключения.</param>
		/// <param name="errHandler">Метод обработки ошибочных ситуаций.</param>
		public AsyncSocketClient(Int32 bufferSize, object stateObject,
		                         MessageHandler msgHandler, SocketClosingHandler closingHandler,
		                         ErrorHandler errHandler)
		{
			_Buffer = bufferSize == 0? new byte[BUFFER_SIZE]: new byte[bufferSize];
			_StateObject = stateObject;

			//Выставление обработчиков
            if (msgHandler != null)
                MessageRecieved += msgHandler;
			if (closingHandler!=null)
			    SocketClosed += closingHandler;
            if (errHandler!= null)
			    TranciverFailed += errHandler;

			//Выставление обработчиков асинхронных методов
			_CallbackReadMethod = new AsyncCallback(ReceiveComplete);
			_CallbackWriteMethod = new AsyncCallback(SendComplete);

			//Мы еще не уничтожены
			_IsDisposed = false;
		}

		/// <summary>
		/// Метод финализации.  если метод Dispose() был корректно вызван, то тут нечего делать
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
		/// Выставляет флаг уничтожения и отсоединяет сокет
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
                //Падение при отключении сокета не принципиально
            }
		}

		/// <summary>
		/// Присоединяется к серверу по заданному адресу
		/// Также вызывает Receive().
		/// </summary>
		/// <param name="IPAddress">Адрес сервера.</param>
		/// <param name="port">Порт сервера.</param>
		public void Connect(String IPAddress, Int16 port)
		{
			try
			{
				//Соединение случаем уже не открыто?
				if (_NetworkStream == null)
				{
					_ServerIPAddress = IPAddress;
					_ServerPort = port;

					//Пытаемся получить соединение
					_TcpClient = new TcpClient(_ServerIPAddress, _ServerPort);
                    InitTcpClient();

					//Начинаем прием сообщений
					Receive();
				}
			}
			catch (SocketException exc)
			{
				//Соединение установить не удалось
				throw new Exception(exc.Message, exc.InnerException);
			}
		}

        /// <summary>
        /// Инициализация клиента
        /// </summary>
        private void InitTcpClient()
        {
            _NetworkStream = _TcpClient.GetStream();
            //Некоторые настройки сокета
            _TcpClient.ReceiveBufferSize = BUFFER_SIZE;
            _TcpClient.SendBufferSize = BUFFER_SIZE;
            _TcpClient.NoDelay = true;
            //Если соединение падает, стираем все ассоциированные данные
            _TcpClient.LingerState = new LingerOption(false, 0);
        }

		///<summary>
		/// Отсоединение
		/// </summary>
		public void Disconnect()
		{
			//Отключаем соединение с предварительной проверкой его существования
			if (_NetworkStream != null)
			{
				_NetworkStream.Close();
			}
			if (_TcpClient != null)
			{
				_TcpClient.Close();
			}

			//Подготовка для сборщика мусора, мы возможно захотим еще раз использовать эти экземпляры
			_NetworkStream = null;
			_TcpClient = null;
            //Соединение закрыто, надо об этом сообщить
            OnSocketClose();
		}

        protected void OnSocketClose()
        {
            if (SocketClosed != null)
                SocketClosed(this);
        }

		///<summary>
		/// Асинхронно передает данные
		/// </summary>
		/// <param name="buffer">
		/// Буфер данных для отправки
		/// </param>
		public void Send(byte[] buffer)
		{
			//отправка данных; не беспокоясь о получении какой-либо информации о состоянии
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
		/// Асинхронный прием данных
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
		/// Вызывается при посылке сообщения
		/// </summary>
		/// <param name="state">Объект состояния, содержащий информацию о соединении.</param>
		private void SendComplete(IAsyncResult state)
		{
			try
			{
				//Проверяем что поток валиден для записи
				if (_NetworkStream.CanWrite)
				{
					_NetworkStream.EndWrite(state);
				}
			}
			catch(Exception ex)
			{
                //Не отправилось - упало, тоже придется подчищать
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
		/// Вызывается при получении сообщения
		/// </summary>
        /// <param name="state">Объект состояния, содержащий информацию о соединении.</param>
		private void ReceiveComplete(IAsyncResult state)
		{
			try
			{
				//Проверяем, что поток валиден для чтения
				if (_NetworkStream.CanRead)
				{
					int bytesReceived = _NetworkStream.EndRead(state);

					//Если есть прочтенные данные, то все путем, иначе сокет поломался
                    // и надо подчистить
					if (bytesReceived > 0)
					{
						try
						{
							//Отправить полученное сообщение обработчику
                            OnMessageRecieved();
						}
						finally
						{
							//Ожидать дальнейших инструкций
							Receive();
						}
					}
				}
			}
			catch (Exception ex)
			{
                //Чтение упало, придется начинать все заново	
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
