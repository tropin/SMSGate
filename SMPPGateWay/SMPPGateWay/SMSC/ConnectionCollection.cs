using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using RoaminSMPP;
using System.Net.Sockets;

namespace Csharper.SMS.SMSC
{
    public class ConnectionCollection: ObservableCollection<AsyncSocketClient>
    {
        public object SyncRoot = new object();
        
        public AsyncSocketClient AddConnection(TcpClient client)
        {
            AsyncSocketClient ASClient = new AsyncSocketClient(client, 0, null);
            ASClient.SocketClosed += ClientTerminated;
            ASClient.TranciverFailed += ClientFailed;
            this.Add(ASClient);
            return ASClient;
        }

        private void RemoveFailedItem(AsyncSocketClient asyncSocketClient)
        {
            lock (SyncRoot)
            {
                if (this.Contains(asyncSocketClient))
                    this.Remove(asyncSocketClient);
            }
        }

        private void ClientFailed(AsyncSocketClient asyncSocketClient, Exception ex)
        {
            RemoveFailedItem(asyncSocketClient);
        }

        private void ClientTerminated(AsyncSocketClient asyncSocketClient)
        {
            RemoveFailedItem(asyncSocketClient);
        }
    }
}
