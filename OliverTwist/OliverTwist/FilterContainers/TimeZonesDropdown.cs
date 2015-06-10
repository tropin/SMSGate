using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OliverTwist.Converters;

namespace OliverTwist.FilterContainers
{
    public class TimeZonesDropdown
    {
        private static TimeZoneConverter cnv = new TimeZoneConverter();
        public static List<SelectListItem> Get(string selected)
        {
            return TimeZoneInfo.GetSystemTimeZones().Select
                (
                    tz =>
                    new SelectListItem
                                {
                                    Text = tz.DisplayName,
                                    Value = cnv.ConvertToString(tz),
                                    Selected = tz == cnv.ConvertFromString(selected)
                                }
                ).ToList();
        }
    }
}