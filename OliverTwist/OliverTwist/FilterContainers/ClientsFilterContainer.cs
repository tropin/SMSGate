using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Csharper.OliverTwist.Model;
using MvcFlan;
using OliverTwist.Converters;
using System.ComponentModel.DataAnnotations;

namespace OliverTwist.FilterContainers
{
    public class ClientsFilterContainer
    {
        public string Login { get; set; }
        public string OrganizationName { get; set; }
        [DataType("Enum")]
        public ClientStatus? Status { get; set; }
        public bool? IsDealler { get; set; }
    }
}