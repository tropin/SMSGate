using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Csharper.OliverTwist.Model
{
    public class ChangeClientAccountModel
    {
        /// <summary>
        /// Идентификатор счета
        /// </summary>
        [ScaffoldColumn(false)]
        public long? Id { get; set; }

        /// <summary>
        /// Выбранный диапазон
        /// </summary>
        [ScaffoldColumn(false)]
        public long? SelectedCostRangeId { get; set; }

        /// <summary>
        /// Доступные диапазоны цен
        /// </summary>
        [DisplayName("Доступные диапазоны цен")]
        public List<CostRangeModel> CostRanges { get; set; }

        /// <summary>
        /// Реальное состояние счета
        /// </summary>
        [DisplayName("Реальное состояние счета")]
        public decimal? Account { get; set; }

        /// <summary>
        /// Добавляемое количество СМС
        /// </summary>
        [Required(ErrorMessage="Количество СМС не может быть не указано")]
        [DisplayName("Добавляемое количество СМС")]
        public decimal? AddingAmount { get; set; }
        
        /// <summary>
        /// Результирующая сумма
        /// </summary>
        [Required(ErrorMessage="Сумма не может быть не указана")]
        [DisplayName("Сумма")]        
        public decimal? InputMoney {get; set;}
        
        /// <summary>
        /// Стоимость одной СМС
        /// </summary>
        [Required(ErrorMessage = "Стоимость СМС не может быть не указана")]
        [DisplayName("Стоимость одной СМС")]
        public decimal? OneSMSCost {get; set;}

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий к операции")]        
        public string Comment { get; set; }

        /// <summary>
        /// Режим рассчета параметров
        /// </summary>
        [DisplayName("Режим рассчета")]
        [DataType("Enum")]
        public CostCalculatorMode CalcMode { get; set; }
    }
}
