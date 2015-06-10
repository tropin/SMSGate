using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcFlan.PagingSorting;
using System.Web.Script.Serialization;

namespace OliverTwist.FilterContainers
{
    public class OptionsHolder<T>
    {
        public T Filter { get; set; }
        public PageSortOptions Sort { get; set; }

        public static OptionsHolder<T> GetHolder(string jsonString)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Deserialize<OptionsHolder<T>>(jsonString);
        }
    }
}