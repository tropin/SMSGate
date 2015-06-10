using System.Diagnostics;

namespace Csharper.SMS.PerformanceCounterManagment
{
	class CounterContainer
	{
		public PerformanceCounter PerformanceCounter { get; private set; }

		public string Name
		{
			get { return PerformanceCounter.CounterName; }
		}

		public string TypeString
		{
			get { return PerformanceCounter.CounterType.ToString(); }
		}

		public CounterContainer(PerformanceCounter performanceCounter)
		{
			PerformanceCounter = performanceCounter;
		}

		public override string ToString()
		{
			return string.Format(@"{0} ({1})", Name, TypeString);
		}
	}
}