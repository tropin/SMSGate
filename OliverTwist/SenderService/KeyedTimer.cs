using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Csharper.SenderService
{
    public class KeyedTimer<T>: Timer
    {
        public T Key {get; private set;}

        public KeyedTimer(TimeSpan interval, T key): base(interval.TotalMilliseconds)
        {
            Key = key;
        }
    }
}
