using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OliverTwist.Converters;
using Csharper.OliverTwist.Repo;

namespace OliverTwist.FilterContainers
{
    public class RolesDropdown
    {
        public static List<SelectListItem> Get(string selected)
        {
            return UserProfileRepo.GetAvailableRoles().Select
                (
                    role =>
                    new SelectListItem
                                {
                                    Text = role,
                                    Value = role,
                                    Selected = role == selected
                                }
                ).ToList();
        }
    }
}