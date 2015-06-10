using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Тип счета
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// Предоплата
        /// </summary>
        [Display(Name = "Предоплата")]
        PrePay = 0,
        /// <summary>
        /// Постоплата
        /// </summary>
        [Display(Name = "Постоплата")]
        PostPay = 1
    }
}
