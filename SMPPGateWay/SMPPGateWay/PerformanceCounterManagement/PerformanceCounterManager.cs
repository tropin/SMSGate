using System.Diagnostics;
using System.Windows.Forms;
using Csharper.SMS.Properties;

namespace Csharper.SMS.PerformanceCounterManagment
{
	public static class PerformanceCounterManager
	{
		public static void Start()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new PerformanceCounterManagerForm());
		}

		public static void CreateCounters()
		{
			CreateCounters(Settings.Default.PerfomanceCounterGroupName);
		}

		public static void CreateCounters(string groupName)
		{
			if (PerformanceCounterCategory.Exists(groupName))
				PerformanceCounterCategory.Delete(groupName);

			CounterCreationDataCollection counters = new CounterCreationDataCollection
				{
					new CounterCreationData(
						@"n_sock_ps",
						@"New socket connections per second",
						PerformanceCounterType.RateOfCountsPerSecond32
						),

					new CounterCreationData(
						@"act_sock_ique",
						@"Active sockets in queue",
						PerformanceCounterType.NumberOfItems32
						),

					new CounterCreationData(
						@"err_sock_ps",
						@"Socket errors per second",
						PerformanceCounterType.RateOfCountsPerSecond32
						),

                    new CounterCreationData(
						@"smpp_cmd_ps",
						@"SMPP commands per second",
						PerformanceCounterType.RateOfCountsPerSecond32
						),

					new CounterCreationData(
						@"smpp_act_conn",
						@"SMPP active connections count",
						PerformanceCounterType.NumberOfItems32
						),

					new CounterCreationData(
						@"smpp_mess_std_ps",
						@"SMPP messages stored per second",
						PerformanceCounterType.RateOfCountsPerSecond32
						),

					new CounterCreationData(
						@"str_mess_ique",
						@"Messages waiting for processing in storage queue",
						PerformanceCounterType.NumberOfItems32
						),

                    new CounterCreationData(
						@"str_item_proc_ps",
						@"Processing messages in storage queue per second",
						PerformanceCounterType.RateOfCountsPerSecond32
						),

					new CounterCreationData(
						@"mess_sent_ps",
						@"Messages totally sent per second",
						PerformanceCounterType.RateOfCountsPerSecond32
						)
						,

					new CounterCreationData(
						@"str_item_proc_dur",
						@"Queue item processing duration",
						PerformanceCounterType.ElapsedTime
						)                                 	
				};

			PerformanceCounterCategory.Create(groupName, groupName,
					PerformanceCounterCategoryType.SingleInstance, counters);
		}
	}
}
