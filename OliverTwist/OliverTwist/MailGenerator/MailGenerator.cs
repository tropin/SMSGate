using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Csharper.OliverTwist.Model;

namespace OliverTwist
{
    public static class MailGenerator
    {
        private static SmtpClient _mailer = null;
        public static SmtpClient Mailer
        {
            get
            {
                if (_mailer == null)
                    _mailer = new SmtpClient();
                return _mailer;
            }
        }
        
        public static MailMessage GetUserConfirmMail(MembershipUser user, RequestContext rcontext)
        {
            UrlHelper urlHepler = new UrlHelper(rcontext);
            string resultingLink = string.Concat(rcontext.HttpContext.Request.Url.GetLeftPart(System.UriPartial.Authority), urlHepler.Action("VerifyUser", "Account", new { userName = user.UserName, vCode = ApproveHasher.GetHash(user) }));
            MailMessage result = new MailMessage();
            result.IsBodyHtml = true;
            result.To.Add(new MailAddress(user.Email));
            result.BodyEncoding = Encoding.UTF8;
            result.Subject = "Регистрация на сервисе ADE.SMS";
            result.Body =
                string.Format(
                @"Вы зарегистрировались на сервисе ADE.SMS, для активации учетной записи пройдите по ссылке <a href=""{0}"">{0}</a><br/> С уважением <br/> Администрация ADE.SMS",
                resultingLink
                );
            return result;
        }

        public static MailMessage GetClientInviteMail(MembershipUser user, string senderName, RequestContext rcontext)
        {
            UrlHelper urlHepler = new UrlHelper(rcontext);
            string resultingLink = string.Concat(rcontext.HttpContext.Request.Url.GetLeftPart(System.UriPartial.Authority),urlHepler.Action("AcceptInviteation", "Account", new { userName = user.UserName, vCode = ApproveHasher.GetHash(user) }));
            MailMessage result = new MailMessage();
            result.IsBodyHtml = true;
            result.To.Add(new MailAddress(user.Email));
            result.BodyEncoding = Encoding.UTF8;
            result.Subject = string.Format("Приглашение в сервис ADE.SMS от участника {0}", senderName);
            result.Body =
                string.Format(
                @"Вас пригласили в качестве клиента в сервис ADE SMS, для подтверждения перейдите по ссылке <a href=""{0}"">{0}</a><br/> С уважением <br/> Администрация ADE.SMS",
                resultingLink
                );
            return result;
        }
    }
}