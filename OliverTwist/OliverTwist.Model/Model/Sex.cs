using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Пол
    /// </summary>
    public enum Sex
    {
        /// <summary>
        /// Женский
        /// </summary>
        [Display(Name = "Женский")]
        Female = 0,
        /// <summary>
        /// Мужской
        /// </summary>
        [Display(Name = "Мужской")]
        Male = 1,
        /// <summary>
        /// Женско-мужской
        /// </summary>
        [Display(Name = "Женский-Мужской")]
        FemaleMale = 2,
        /// <summary>
        /// Мужско-женский
        /// </summary>
        [Display(Name = "Мужской-Женский")]
        MaleFemale = 3,
        /// <summary>
        /// Неизвестный
        /// </summary>
        [Display(Name = "Неопределенный")]
        Unknown = 4
    }
}
