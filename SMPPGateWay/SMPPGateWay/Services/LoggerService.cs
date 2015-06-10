using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Csharper.SMS.Services
{
    public static class LoggerService
    {
        private static TraceSource _logger = null; 
        
        public static TraceSource Logger
        {
            get
            {
                return GetLogger();
            }
        }

        private static TraceSource GetLogger()
        {
            if (_logger == null)
                _logger = new TraceSource("SMPPGateway");
            return _logger;
        }
    }
}
