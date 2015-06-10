using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Модель диапазонов цен для счета
    /// </summary>
    public class CostRangeModel
    {
        /// <summary>
        /// Идентификатор диапазона
        /// </summary>
        [ScaffoldColumn(false)]
        public long? Id { get; set; }
        /// <summary>
        /// Количество смс для активации цены
        /// </summary>
        [ScaffoldColumn(false)]
        [DisplayName("Количество смс для активации цены")]
        public long Volume { get; set; }
        
        /// <summary>
        /// Цена
        /// </summary>
        [ScaffoldColumn(false)]
        [DisplayName("Цена")]
        public decimal Cost { get; set; }

        /// <summary>
        /// Минимальная сумма для активации предложения
        /// </summary>
        [DisplayName("Минимальная сумма для активации предложения")]
        [ScaffoldColumn(false)]
        [Editable(false)]
        public decimal? LowerSum { get; set; }
    }
}
