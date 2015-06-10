using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Модель счета клиента
    /// </summary>
    public class ClientAccountModel
    {
        /// <summary>
        /// Идентификатор счета
        /// </summary>
        [ScaffoldColumn(false)]
        public long? Id { get; set; }

        /// <summary>
        /// Доступные диапазоны цен
        /// </summary>
        [DisplayName("Доступные диапазоны цен")]
        public List<CostRangeModel> CostRanges { get; set; }
        
        /// <summary>
        /// Счет
        /// </summary>
        [DisplayName("Счет")]
        public decimal Account { get; set; }
        
        /// <summary>
        /// Баланс
        /// </summary>
        [DisplayName("Баланс")]
        public decimal Ballance { get; set; }
        
        /// <summary>
        /// Тип счета
        /// </summary>
        [DisplayName("Тип счета")]        
        [DataType("Enum")]
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Тип списания
        /// </summary>
        [DisplayName("Тип списания")]
        [DataType("Enum")]
        public DebtingType DebtingType { get; set; }
    }
}
