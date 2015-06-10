using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OliverTwist.FilterContainers
{
    public class StatisticsFilterContainer
    {
        [TypeConverter(typeof(DateTimeConverter))]
        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }
        [TypeConverter(typeof(DateTimeConverter))]
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }
        public Guid? UserId { get; set; }
        public long? ClientId { get; set; }
    }
}