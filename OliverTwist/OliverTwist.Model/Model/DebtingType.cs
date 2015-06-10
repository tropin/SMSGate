using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Тип списания
    /// </summary>
    public enum DebtingType
    {
        /// <summary>
        /// По отправленым
        /// </summary>
        [Display(Name = "По отправленым")]
        BySent = 0,
        /// <summary>
        /// По доставленым
        /// </summary>
        [Display(Name = "По доставленым")]
        ByDelivered = 1
    }
}
