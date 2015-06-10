using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS.Services
{
    public enum LoggingCatoegory
    {
         /// <summary>
        /// Протокол обмена
        /// </summary>
        Protocol = 1,
        /// <summary>
        /// Хранение очереди сообщений
        /// </summary>
        Storage = 2,
        /// <summary>
        /// Обработка очереди сообщений
        /// </summary>
        Processing = 3,
        /// <summary>
        /// Низкоуровневая работа с сетью
        /// </summary>
        Network = 4
    }
}
