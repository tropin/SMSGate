using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Csharper.OliverTwist.Repo;
using Csharper.OliverTwist.Model;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Diagnostics;

namespace OliverTwist.Controllers
{

    [HandleError]
    public class AccountController: Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        private IClientRepo _clientRepo = null;
        public IClientRepo ClientRepo 
            {
                get
                {
                    if (_clientRepo == null)
                        TryInitClientRepo();
                    return _clientRepo;
                }
                set
                {
                    _clientRepo = value;
                }
            }

        private void TryInitClientRepo()
        {
             ClientRepo = RepoGetter<ClientRepo>.Get(string.Empty, null, null);
        }

        protected override void Initialize(RequestContext requestContext)
        {            
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [NonAction]
        private bool VerifyUserInternal(string userName, string vCode, bool activate = true)
        {
            FormsService.SignOut();
            bool result = false;
            MembershipUser user = MembershipService.GetUser(userName);
            if (user != null)
            {
                if (!user.IsApproved && !user.IsLockedOut && ApproveHasher.IsValidHash(user, vCode))
                {
                    if (activate)
                        MembershipService.ActivateUser(user);
                    FormsService.SignIn(userName, false);
                    ClientRepo.ActivateClientForUser((Guid)user.ProviderUserKey);
                    result = true;
                }
            }
            return result;
        }
        
        [ValidateInput(false)]
        public ActionResult VerifyUser(string userName, string vCode)
        {
            VerifyUserInternal(userName, vCode);
            return RedirectToAction("Index", "Home");
        }
        
        [ValidateInput(false)]
        public ActionResult AcceptInviteation(string userName, string vCode)
        {
            ActionResult result = RedirectToAction("Index", "Home");
            if (VerifyUserInternal(userName, vCode, activate: false))
            {
                result = ResetPassword();
            }
            return result;
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult AcceptInviteation(ChangePasswordModel model)
        {
            MembershipUser user = MembershipService.GetUser(User.Identity.Name);
            if (user!=null && !user.IsApproved && ModelState.IsValid)
            {
                bool isSuccess = MembershipService.ResetPassword(User.Identity.Name, model.NewPassword);
                MembershipService.ActivateUser(user);
                if (isSuccess)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Новый пароль некорректен.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Имя или пароль неверны.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //Если пользователь зарегистрировался, то для своего клиента он Админ
                    Roles.AddUserToRole(model.UserName, RoleNames.ADMIN);
                    //Создаем клиента
                    MembershipUser user = Membership.GetUser(model.UserName);
                    ClientModel client = ClientRepo.CreateClient(model,model.OrganizationName, user);
                    bool isMailError = false;
                    if (client != null)
                    {
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        try
                        {
                            MailGenerator.Mailer.Send(MailGenerator.GetUserConfirmMail(user, Request.RequestContext));
                        }
                        catch (Exception ex)
                        {
                            isMailError = true;
                            Trace.TraceError("Ошибка отправки уведомления об отправке пользователь {0} email {1}, ошибка {2}", user.UserName, user.Email, ex);
                        }
                    }
                    return RedirectToAction("Index", "Home", isMailError ? new { User = user, Client = client } : null);
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [NonAction]
        public ActionResult ResetPassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View("ChangePassword", new ChangePasswordModel());
        }
        
        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(new ChangePasswordWithOldModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangePassword(ChangePasswordWithOldModel model)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess = MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                if (isSuccess)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Текущий пароль не верен или новый пароль некорректен.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}
