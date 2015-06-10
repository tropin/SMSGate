using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcFlan.Attributes;

namespace Csharper.OliverTwist.Model
{

    #region Models
    [PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
    public class ChangePasswordModel
    {   
        [Required(ErrorMessage = "Новый пароль обязателен")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Новый пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтверждение нового пароля обязательно")]
        [DataType(DataType.Password)]
        [DisplayName("Подтверждение нового пароля")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordWithOldModel : ChangePasswordModel
    {
        [Required(ErrorMessage = "Текущий пароль обязателен")]
        [DataType(DataType.Password)]
        [DisplayName("Текущий пароль")]
        public string OldPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [DisplayName("Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Пароль не может быть пустым")]
        [DataType(DataType.Password)]
        [DisplayName("Пароль")]
        public string Password { get; set; }

        [DisplayName("Помнишь меня?")]
        public bool RememberMe { get; set; }
    }

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
    public class RegisterModel: UserProfileModel
    {
        /// <summary>
        /// Наименование организации
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Наименование организации является обязательным")]
        [DisplayName("Наименование организации")]
        public string OrganizationName { get; set; }
        
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Имя пользователя пустое")]
        [DisplayName("Имя пользователя")]
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пустой пароль недопустим")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Пароль")]
        public string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Required(ErrorMessage = "Подтвержение пароля обязательно")]
        [DataType(DataType.Password)]
        [DisplayName("Подтверждение пароля")]
        public string ConfirmPassword { get; set; }
    }
    #endregion

    #region Services
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IMembershipService
    {
        int MinPasswordLength { get; }
        int MinRequiredNonAlphanumericCharacters { get; }
        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email, bool isApproved = false, string secretQuestion = null, string secretAnswer = null);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool ResetPassword(string userName, string newPassword);
        void ActivateUser(MembershipUser user);
        MembershipUser GetUser(string userName);
        MembershipUser GetUser();
        bool DeleteUser(string userName);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return _provider.MinRequiredNonAlphanumericCharacters;
            }
        }

        public bool DeleteUser(string userName)
        {
            return _provider.DeleteUser(userName, true);
        }

        public void ActivateUser(MembershipUser user)
        {
            user.IsApproved = true;
            _provider.UpdateUser(user);
        }

        public MembershipUser GetUser(string userName)
        {
            return _provider.GetUser(userName, false);
        }

        public MembershipUser GetUser()
        {
            return Membership.GetUser();
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email, bool isApproved = false, string secretQuestion = null, string secretAnswer = null)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, secretQuestion, secretAnswer, isApproved, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

        public bool ResetPassword(string userName, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Значение параметра не может быть null или пустым.", "userName");
            bool result = false;
            MembershipUser user = _provider.GetUser(userName, true /* userIsOnline */);
            if (user != null)
            {
                string pass = Membership.Provider.ResetPassword(userName, string.Empty);
                result = ChangePassword(userName, pass, newPassword);
            }
            return result;
        }        
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Такое имя уже существует. Попробуйте выбрать другое имя пользователя.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Такой e-mail уже существует. Пожалуйста введите другой e-mail.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Введенный пароль неверен. Введите корректный пароль.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Введенный e-mail неверен, пожалуйста проверьте его написание.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Ответ на секретный вопрос неверен. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Секретный вопрос неверен. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Имя пользователя содержит недопустимое значение.";

                case MembershipCreateStatus.ProviderError:
                    return "Провайдер аутентификации возвратил ошибку. Если проблема повторится, обратитесь к администратору.";

                case MembershipCreateStatus.UserRejected:
                    return "Процесс создания пользователя отменен. Если проблема повторится, обратитесь к администратору";

                default:
                    return "Возникла неизвестная ошибка. Если проблема повторится, обратитесь к администратору.";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' и '{1}' не совпадают.";
        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }
        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            return Object.Equals(originalValue, confirmValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "значение '{0}' должно быть как минимум динной {1} строк.";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }
    }
    #endregion

}
