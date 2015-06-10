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
    public class UserProfilesFilterContainer
    {
        [DisplayName("Логин")]
        public string Login { get; set; }
        [DisplayName("Имя клиента(организации)")]
        public string ClientName { get; set; }
        [DisplayName("Дата создания")]
        public DateTime? CreateDate { get; set; }
        [DisplayName("Подтвержден")]
        public bool? IsApproved { get; set; }
        [DisplayName("Заблокирован")]
        public bool? IsLockedOut { get; set; }
        [DataType("Roles")]
        [DisplayName("Роль")]
        public string Roles { get; set; }
        [DisplayName("Электронная почта")]
        public string Email { get; set; }
        [DisplayName("Фамилия")]
        public string LastName { get; set; }
        [DisplayName("Город")]
        [DataType("City")]
        public string City { get; set; }
        [DisplayName("Номер мобильного телефона")]
        public string MobilePhone { get; set; }
        [DisplayName("Временная зона")]
        [DataType("TimeZone")]
        public string TimeZone { get; set; }
        [DisplayName("Пол")]
        [DataType("Enum")]
        public Sex? Sex { get; set; }
        [DisplayName("Уровень")]
        [DataType("UserChildLevel")]
        public int? ChildLevel { get; set; }
    }
}