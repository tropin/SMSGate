using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.Common
{
    public interface IPagedResult
    {
        /// <summary>
        /// Текущая строка
        /// </summary>
        long? Row { get; set; }
        /// <summary>
        /// Количество записей во всем наборе
        /// </summary>
        int? Records { get; set; }
    }
}
