using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Описание привязки столбцов к имеющемуся содержимому импортируемого файла
    /// </summary>
    public class FileColumnsMap
    {
        /// <summary>
        /// Доступные для привязки колонки
        /// </summary>
        public List<string> AvailableColumns { get; private set; }
        /// <summary>
        /// Доступные данные в файле в виде таблицы 
        /// </summary>
        public List<List<string>> Items { get; private set; }

        public FileColumnsMap()
        {
            AvailableColumns = new List<string>();
            Items = new List<List<string>>();
        }

        public string[] Separators { get; set; }
    }
}
