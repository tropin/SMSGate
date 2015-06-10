using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.OliverTwist.Model
{
    public static class ImportPolicy
    {
        /// <summary>
        /// Максимальный размер файла загружаемого на сервер
        /// </summary>
        public static long MaxFileSize
        {
            get
            {
                return 41943040; //40 мегабайт
            }
        }

        /// <summary>
        /// Максимальное количество загружаемых файлов в единицу времени
        /// </summary>
        public static long MaxFileCountPerTimeRange
        {
            get
            {
                return 4; //4 файла в NextFilePackageLoadTimeRange 
            }
        }

        /// <summary>
        /// Время в которое дозволено MaxFileCountPerTimeRange загрузок файлов не более MaxFileSize байт
        /// </summary>
        public static TimeSpan NextFilePackageLoadTimeRange
        {
            get
            {
                return TimeSpan.FromHours(0.5d); //Полчаса
            }
        }

        /// <summary>
        /// Время которое ждем до следующей загрузки, если спамим
        /// </summary>
        public static TimeSpan LoadOversizeWaitTime
        {
            get
            {
                return TimeSpan.FromHours(0.5d); //Полчаса
            }
        }
    }
}
