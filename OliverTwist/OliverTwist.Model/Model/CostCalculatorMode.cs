using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Режим работы калькулятора стоимостей
    /// </summary>
    public enum CostCalculatorMode
    {
        /// <summary>
        /// Режим фиксированной суммы
        /// </summary>
        [Display(Name = "Режим фиксированной суммы")]
        FixedMoney = 0,
        /// <summary>
        /// Режим фиксированного количества СМС
        /// </summary>
        [Display(Name = "Режим фиксированного количества СМС")]
        FixedSMSAmount = 1,
        /// <summary>
        /// Режим фиксированной цены СМС
        /// </summary>
        [Display(Name = "Режим фиксированной цены СМС")]
        FixedCost = 2
    }
}