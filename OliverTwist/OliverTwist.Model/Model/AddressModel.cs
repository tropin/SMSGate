using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model.Properties;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MvcFlan.Attributes;
using OliverTwist.Converters;
using System.Diagnostics;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Модель записи адресной книги
    /// </summary>
    public class AddressModel
    {
        /// <summary>
        /// Идентификатор записи адресной книги
        /// </summary>
        [ScaffoldColumn(false)]
        [OrderBy(IsDefault=true)]
        public long? Id { get; set; }
        
        /// <summary>
        /// Имя
        /// </summary>
        [DisplayName("Имя")]
        public string FirstName {get; set;}

        /// <summary>
        /// Отчество
        /// </summary>
        [DisplayName("Отчество")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [SearchFilter]
        [DisplayName("Фамилия")]
        public string LastName { get; set;}

        /// <summary>
        /// Пол
        /// </summary>
        [SearchFilter]
        [DisplayName("Пол")]
        public Sex? Sex { get; set; }

        /// <summary>
        /// День рождения
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DisplayName("День рождения")]
        public DateTime? BirthDay { get; set; }

        /// <summary>
        /// Номер мобильного телефона
        /// </summary>
        [SearchFilter(FilterType.Contains)]
        [OrderBy]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Номер мобильного телефона является обязательным")]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Номер мобильного телефона")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// Город
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DisplayName("Город")]
        [DataType("City")]
        public string City { get; set; }

        /// <summary>
        /// Описание к записи
        /// </summary>
        [SearchFilter(FilterType.Contains)]
        [DisplayName("Описание")]
        public string Description { get; set; }

        /// <summary>
        /// Временная зона
        /// </summary>
        [SearchFilter]
        [DataType("TimeZone")]
        [DisplayName("Временная зона")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Учавствует в группах
        /// </summary>
        [ScaffoldColumn(false)]
        [DisplayName("Группы")]
        public Dictionary<long, string> InGroups { get; set; }
    }
}                                                
