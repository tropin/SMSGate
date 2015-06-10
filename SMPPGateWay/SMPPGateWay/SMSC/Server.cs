using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections.ObjectModel;
using RoaminSMPP;
using System.Collections.Specialized;

namespace Csharper.SMS.SMSC
{
    public class Server
    {
        /// <summary>
        /// Вызывается, устанавливается новое соединение.
        /// </summary>
        /// <param name="asyncSocketClient">
        /// Клиент который отвечает за соединенеие.
        /// </param>
        public delegate void СonnectionHandler(IEnumerable<AsyncSocketClient> asyncSocketClient);
        
        private ConnectionCollection openedConnections = new ConnectionCollection();
        private IPAddress _serverInterface = IPAddress.Any;
        private int _serverPort = 0;
        private bool _isServerRunning = false;

        private static TcpListener _listener = null;

        /// <summary>
        /// Событие получения нового соединения
        /// </summary>
        public event СonnectionHandler ClientConnected;  
        
        /// <summary>
        /// Интерфейс на котором хостится сервер
        /// </summary>
        public IPAddress ServerInterface
        {
            get
            {
                return _serverInterface;
            }
        }

        /// <summary>
        /// Порт на котором хостится сервер
        /// </summary>
        public int ServerPort
        {
            get
            {
                return _serverPort;
            }
        }

        /// <summary>
        /// Признак жизни сервера
        /// </summary>
        public bool IsServerRunning
        {
            get
            {
                return _isServerRunning;
            }
        }

        /// <summary>
        /// Сервер SMSC
        /// </summary>
        public Server()
        {
            openedConnections.CollectionChanged += OnConnectionRecieved;
        }

        void OnConnectionRecieved(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (ClientConnected != null)
                    ClientConnected(e.NewItems.OfType<AsyncSocketClient>());
            }
        }

        /// <summary>
        /// Сервер SMSC на конкретном интерфейсе и на конкретном порту
        /// </summary>
        /// <param name="serverInterface">Интерфейс</param>
        /// <param name="serverPort">Порт</param>
        public Server(IPAddress serverInterface, int serverPort): this()
        {
            _serverInterface = serverInterface;
            _serverPort = serverPort;
        }
        
        /// <summary>
        /// Флаг завершения работы сервера
        /// </summary>
        private volatile bool _stopFlag = false;
        

        /// <summary>
        /// Событие обработки подключения
        /// </summary>
        private AutoResetEvent connectionWaitHandle = new AutoResetEvent(false);
        
        /// <summary>
        /// Событие остановки сервера
        /// </summary>
        private AutoResetEvent serverStopWaitHandle = new AutoResetEvent(false);
        
        /// <summary>
        /// Запуск сервера.
        /// </summary>
        public void Start()
        {
            Thread thread = new Thread(ListenerStart);
            thread.Start();
        }

        /// <summary>
        /// Метод запуска точки подключения
        /// </summary>
        /// <remarks>
        /// Может падать при некорректных настройках среверного интерфейса и порта
        /// </remarks>
        /// <exception cref="System.Net.Sockets.Exception"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.OutOfRangeException"/>
        private void ListenerStart()
        {
            if (_listener == null)
                _listener = new TcpListener(_serverInterface, _serverPort);
            try
            {
                _listener.Start();
                IPEndPoint endPoint = (IPEndPoint)_listener.Server.LocalEndPoint;
                _serverInterface = endPoint.Address;
                _serverPort = endPoint.Port;
                while (!_stopFlag)
                {
                    try
                    {
                        //Ожидаем подключения. т.е. постановки в очередь заявки на подключение.
                        IAsyncResult result = _listener.BeginAcceptTcpClient(HandleAsyncConnection, _listener);
                        //Ждем пока получим сигнал из дочернего потока об успешной инциализации подключения
                        //Или же ждем сигнал о том, что пора сворачиваться
                        _isServerRunning = true;
                        WaitHandle.WaitAny(new[] { connectionWaitHandle, serverStopWaitHandle });
                    }
                    catch (Exception ex)
                    {
                        if (ex is SocketException || ex is ObjectDisposedException)
                        {
                           //Упал слушатель, надо реинициализировать
                            _listener = new TcpListener(_serverInterface, _serverPort);
                            _listener.Start();
                           //Если еще раз упадет, то делать нечего, что-то не так, падаем
                        }
                        //Здесь мы падать не должны. Сервер должен слушать всегда, пока ему не прикажут обратное 
                    }
                }
            }
            finally
            {
                _listener.Stop();
            }
        }

        private void HandleAsyncConnection(IAsyncResult result)
        {
            TcpListener listener = (TcpListener)result.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(result);
            connectionWaitHandle.Set(); //Информируем основной поток, что можно обрабатывать следующее подключение
            openedConnections.AddConnection(client);
        }

        public void Stop()
        {
            _stopFlag = true;
            serverStopWaitHandle.Set(); //Сообщаем, что конец песне
            lock (openedConnections.SyncRoot)
            {
                foreach (AsyncSocketClient client in openedConnections)
                {
                    client.Disconnect();
                }
            }
            _isServerRunning = false;
        }
    }
}
