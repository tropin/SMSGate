using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Csharper.OliverTwist.Model;

namespace OliverTwist.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(MembershipUser user, ClientModel client)
        {
            ViewData["InvalidSendUserName"] = user.UserName;
            ViewData["InvalidSendEmail"] = user.Email;
            ViewData["InvalidSendClientName"] = client.OrganizationName;
            ViewData["InvalidSendClientId"] = client.Id;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
