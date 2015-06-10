using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Csharper.OliverTwist.Model;
using MvcFlan;
using OliverTwist.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OliverTwist.FilterContainers
{
    public class AddressFilterContainer
    {
        [DataType("City")]
        public string City { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        [DataType("Enum")]
        public Sex? Sex { get; set; }
        [TypeConverter(typeof(JSONStringConverter))]      
        public long[] SelectedGroups { get; set; }
    }
}