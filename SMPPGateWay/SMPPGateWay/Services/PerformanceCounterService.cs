using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Csharper.SMS.Properties;

namespace Csharper.SMS.Services
{
    public static class PerformanceCountersService
    {
        private static readonly List<PerformanceCounter> _performanceCounters;
		private static readonly object _syncRoot;

		static PerformanceCountersService()
		{
			_performanceCounters = new List<PerformanceCounter>();
			_syncRoot = new object();
		}

		public static PerformanceCounter GetCounter(string name)
		{
			lock(_syncRoot)
			{
				PerformanceCounter performanceCounter = _performanceCounters.SingleOrDefault(x => x.CounterName == name);

				if (performanceCounter == null)
				{
					performanceCounter = new PerformanceCounter(Settings.Default.PerfomanceCounterGroupName, name, false);

					_performanceCounters.Add(performanceCounter);
				}

				return performanceCounter;
			}
		}
    }
}
