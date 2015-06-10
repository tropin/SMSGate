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
    /// Модель клиента
    /// </summary>
    public class ClientModel
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        [ScaffoldColumn(false)]
        public long? Id { get; set; }

        /// <summary>
        /// Имена входа привязанные к клиенту
        /// </summary>
        [TypeConverter(typeof(ListOfStringConverter))]
        [Editable(false)]
        [DisplayName("Логин")]
        public List<string> Login { get; set; }

        /// <summary>
        /// Диапазоны цен
        /// </summary>
        [TypeConverter(typeof(JSONStringConverter))]
        [DisplayName("Диапазоны цен")]
        [DataType("CostRanges")]
        [ScaffoldColumn(false)]
        public List<CostRangeModel> CostRanges { get; set; }
        
        /// <summary>
        /// Наименование организации
        /// </summary>
        [SearchFilter(FilterType.Contains)]
        [OrderBy(IsDefault = true)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Наименование организации является обязательным")]
        [DisplayName("Наименование организации")]
        public string OrganizationName { get; set; }
       

        /// <summary>
        /// Статус клиента
        /// </summary>
        [SearchFilter]
        [DataType("Enum")] 
        [DisplayName("Статус клиента")]
        public ClientStatus? Status { get; set; }

        /// <summary>
        /// Диллер
        /// </summary>
        [OrderBy]
        [SearchFilter]
        [DisplayName("Диллер")]
        public bool? IsDealler { get; set; }

        /// <summary>
        /// Счет
        /// </summary>
        [OrderBy]
        [ScaffoldColumn(false)]
        [DisplayName("Счет")]
        public decimal? Account { get; set; }
        
        /// <summary>
        /// Баланс
        /// </summary>
        [OrderBy]
        [Editable(false)]
        [DisplayName("Балланс")]
        public decimal? Ballance {get; set;}


        /// <summary>
        /// Баланс
        /// </summary>
        [OrderBy]
        [DataType("Enum")] 
        [DisplayName("Тип списания")]
        public DebtingType DebtingType { get; set; }

    }
}                                                
