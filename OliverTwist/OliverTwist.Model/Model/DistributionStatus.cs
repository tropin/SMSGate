using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Статус рассылки
    /// </summary>
    public enum DistributionStatus
    {
        /// <summary>
        /// Создана
        /// </summary>
        [Display(Name = "Создана")]
        Created = 0,
        /// <summary>
        /// Выполняется
        /// </summary>
        [Display(Name = "Выполняется")]
        Running = 1,
        /// <summary>
        /// Выполнена
        /// </summary>
        [Display(Name = "Выполнена")]
        Completed = 2,
        /// <summary>
        /// Отменена
        /// </summary>
        [Display(Name = "Отменена")]
        Cancelled = 3
    }
}
