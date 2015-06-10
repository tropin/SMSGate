using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.Common;

namespace Csharper.OliverTwist.Model
{
    public class StatisticsModel
    {
        /// <summary>
        /// Счетчики
        /// </summary>
        public List<SMSCounter> Counters { get; set; }
        
        /// <summary>
        /// Детализация
        /// </summary>
        public List<SMSDetail> Details { get; set; }

        /// <summary>
        /// Информация о паджинации
        /// </summary>
        public SPPaginator Paging { get; set; }
    }
}
