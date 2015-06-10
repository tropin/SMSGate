using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Csharper.SenderService
{
    public class ConnectionItem
    {
        public RoaminSMPP.SMPPCommunicator Connection { get; set; }
        public Timer ConnectionRefreshTimer { get; set; }
        public int ReconnectAttempts { get; set; }

        private object _syncRoot = new object();
        private bool _isRunning = false;

        public bool IsRunning
        {
            get
            {
                lock (_syncRoot)
                {
                    bool isRunning = _isRunning;
                    if (!_isRunning)
                        _isRunning = true;
                    return isRunning;
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    _isRunning = value;
                }
            }
        }
    }
}
