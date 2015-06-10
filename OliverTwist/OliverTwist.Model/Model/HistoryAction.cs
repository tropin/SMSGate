using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Тип изменения, отраженный в истории
    /// </summary>
    public enum HistoryAction
    {
        /// <summary>
        /// Добавление
        /// </summary>
        [Display(Name = "Добавление")]
        Add = 0,
        /// <summary>
        /// Изменение
        /// </summary>
        [Display(Name = "Изменение")]
        Change = 1,
        /// <summary>
        /// Удаление
        /// </summary>
        [Display(Name = "Удаление")]
        Delete = 2
    }
}
