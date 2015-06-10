using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OliverTwist.Converters;
using Csharper.OliverTwist.Repo;

namespace OliverTwist.FilterContainers
{
    public class UserChildLevelDropdown
    {
        public static List<SelectListItem> Get(string selected)
        {
            Dictionary<string,int> deeps = new Dictionary<string,int>();
            deeps.Add("Мои пользователи",0);
            deeps.Add("Пользователи моих клиентов",1);
            deeps.Add("Пользователи клиентов моих клиентов",2);
            deeps.Add("Мои и пользователи моих клиентов",-1);
            deeps.Add("Мои и пользователи клиентов моих клиентов", -2);
            return deeps.Select
                (
                    deep =>
                    new SelectListItem
                                {
                                    Text = deep.Key,
                                    Value = deep.Value.ToString(),
                                    Selected = deep.Value.ToString() == selected
                                }
                ).ToList();
        }
    }
}