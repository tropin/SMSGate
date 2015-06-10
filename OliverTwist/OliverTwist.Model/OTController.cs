using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Csharper.OliverTwist.Model
{
    public class OTController : Controller
    {
        private SessionAcessor sessionAcessor = null;
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
            sessionAcessor = SessionAcessor.GetAcessor(MembershipService);
            base.Initialize(requestContext);
        }
        
        public SessionAcessor OTSession
        {
            get
            {
                return sessionAcessor;
            }
        }
    }
}
