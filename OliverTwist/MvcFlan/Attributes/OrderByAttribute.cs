using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcFlan.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OrderByAttribute : Attribute
    {
        public OrderByAttribute()
            : this(SortOrder.Ascending, false) { }

        public OrderByAttribute(SortOrder sortOrder, bool isDefault)
        {
            this.SortOrder = sortOrder;
            this.IsDefault = IsDefault;
        }

        public SortOrder SortOrder { get; set; }
        public bool IsDefault { get; set; }
    }

}