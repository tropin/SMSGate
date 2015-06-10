using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Статус клиента
    /// </summary>
    public enum ClientStatus
    {
        /// <summary>
        /// Неактивен
        /// </summary>
        [Display(Name = "Неактивен")]
        NotActive = 0,
        /// <summary>
        /// Активен
        /// </summary>
        [Display(Name = "Активен")]
        Active = 1,
        /// <summary>
        /// Прошел полную проверку
        /// </summary>
        [Display(Name = "Прошел полную проверку")]
        Validated = 2,
        /// <summary>
        /// Отключен
        /// </summary>
        [Display(Name = "Отключен")]
        Disabled = 3
    }
}
