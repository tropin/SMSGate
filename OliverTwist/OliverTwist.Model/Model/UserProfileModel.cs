using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcFlan.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using OliverTwist.Converters;

namespace Csharper.OliverTwist.Model
{
    public class UserProfileModel
    {
        /// <summary>
        /// Идентификатор профиля
        /// </summary>
        [ScaffoldColumn(false)]
        [Editable(false)]
        public long? Id { get; set; }

        /// <summary>
        /// Идентификатор профиля
        /// </summary>
        [ScaffoldColumn(false)]
        [Editable(false)]
        public long? ClientId { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        [SearchFilter(FilterType.Contains)]
        [OrderBy(SortOrder = SortOrder.Ascending)]
        [Editable(false)]
        [DisplayName("Имя клиента(Организации)")]
        public string ClientName { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [ScaffoldColumn(false)]
        [Editable(false)]
        public Guid UserId { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        [SearchFilter(FilterType.Contains)]
        [OrderBy(SortOrder = SortOrder.Ascending, IsDefault=true)]
        [Editable(false)]
        [DisplayName("Логин")]
        public string Login { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [OrderBy]
        [SearchFilter]
        [Editable(false)]
        [DisplayName("Дата создания")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Подтвержден
        /// </summary>
        [OrderBy]
        [SearchFilter]
        [DisplayName("Подтвержден")]
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Заблокирован
        /// </summary>
        [OrderBy]
        [SearchFilter]
        [DisplayName("Заблокирован")]
        public bool? IsLockedOut { get; set; }

        /// <summary>
        /// Роли
        /// </summary>
        [TypeConverter(typeof(ListOfStringConverter))]
        [DisplayName("Роли")]
        public List<string> Roles { get; set; }


        /// <summary>
        /// Электронная почта
        /// </summary>
        [SearchFilter(FilterType.Contains)]
        [OrderBy]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Электронная почта является обязательной")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Электронная почта")]
        public string Email { get; set; }
        
        /// <summary>
        /// Имя
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Имя является обязательным")]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчетсво
        /// </summary>
        [DisplayName("Отчетсво")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [SearchFilter(FilterType = FilterType.Contains)]
        [OrderBy]
        [DisplayName("Фамилия")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Фамилия является обязательной")]
        public string LastName { get; set; }

        /// <summary>
        /// Город
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DisplayName("Город")]
        [DataType("City")]
        public string City { get; set; }

        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        [SearchFilter(FilterType.Contains)]
        [OrderBy]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Номер мобильного телефона")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// Временная зона
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DataType("TimeZone")]
        [DisplayName("Временная зона")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DataType("Enum")]
        [DisplayName("Пол")]
        public Sex? Sex { get; set; }
    }
}
