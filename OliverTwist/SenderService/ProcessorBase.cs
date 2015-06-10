using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Csharper.SenderService.DAL;

namespace Csharper.SenderService
{
    public abstract class ProcessorBase<T>
        where T: Timer, new()
    {
        private T _timer;

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

        private SenderShedullerEntities _context;

        protected SenderShedullerEntities Context
        {
            get
            {
                if (_context == null)
                    _context = new SenderShedullerEntities();
                return _context;
            }
        }

        public ProcessorBase(TimeSpan timeInterval)
        {
            _timer = new T();
            _timer.Interval = timeInterval.TotalMilliseconds;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsRunning)
            {
                try
                {
                    Process(sender as T);
                }
                finally
                {
                    IsRunning = false;
                }
            }
        }

        protected abstract void Process(T timer);
    }
}
