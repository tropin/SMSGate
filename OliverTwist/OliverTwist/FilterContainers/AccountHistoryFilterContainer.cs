using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OliverTwist.FilterContainers
{
    public class AccountHistoryFilterContainer
    {
        public DateTime? VersionDate { get; set; }

        public string ManagerName { get; set; }

        public string RealClientName { get; set; }

        public string OperationalClientName { get; set; }

        public string TargetAccountOrganizationName { get; set; }

        public decimal? MoneyVolume { get; set; }

        public decimal? QuickCost { get; set; }
    }
}