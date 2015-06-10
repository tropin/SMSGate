using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Csharper.SMSServiceGate
{
    public struct SessinInfo
    {
        /// <summary>
        /// Уникальный хеш составленый из логина и пароля
        /// </summary>
        public string UniquHash;
        /// <summary>
        /// Сессионный ключ с сервиса
        /// </summary>
        public string SessionKey;
        /// <summary>
        /// Время когда сессию стоит счиать мертвой
        /// </summary>
        public DateTime TimeToKill;
    }
}