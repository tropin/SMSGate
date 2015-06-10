using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Csharper.SMS.SMSC;
using System.Net;
using Csharper.SMS.Properties;
using RoaminSMPP;
using Csharper.SMS.Storage;
using Csharper.SMS.Billing;

namespace Csharper.SMS
{
    public partial class SMPPGateWay : ServiceBase
    {
        private static Server _server = null;
        private const string GATEWAY_LOGGER_SOURCE_NAME = "SMPPGateway";
        
        public SMPPGateWay()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (_server == null)
                _server = new Server(IPAddress.Parse(Settings.Default.Interface), Settings.Default.Port);
            if (!_server.IsServerRunning)
            {
                _server.Start();
                _server.ClientConnected += new Server.СonnectionHandler(ConnectionRecieved);
            }
        }

        protected override void OnStop()
        {
            if (_server != null && _server.IsServerRunning)
                _server.Stop();
            _server = null;
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (_server!=null && _server.IsServerRunning)
                _server.Stop();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            if (_server!=null && !_server.IsServerRunning)
                _server.Start();
        }

        private void ConnectionRecieved(IEnumerable<AsyncSocketClient> asyncSocketClient)
        {
            AsyncSocketClient client = asyncSocketClient.FirstOrDefault();
            if (client != null)
            {
                ServerProcessor processor = new ServerProcessor(client, new DataBaseStorageProvider(), new DataBaseBillingProvider());
            }
        }
    }
}
