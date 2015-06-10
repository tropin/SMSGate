using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Csharper.Common;
using Csharper.OliverTwist.Model;

namespace Csharper.OliverTwist.Services
{
    /// <summary>
    /// Сессия пользователя на клиентском сервисе
    /// </summary>
    public class ServiceSession: ISession
    {
        /// <summary>
        /// Сессионный ключ
        /// </summary>
        public string SessionKey { get; set; }
        /// <summary>
        /// Логин
        /// </summary>
        [SessionUnique]
        public string Login { get; set; }

        /// <summary>
        /// Имя отправителя
        /// </summary>
        [SessionUnique]
        public string SenderName { get; set; }

        /// <summary>
        /// Идентификатор клиента в системе
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Тип списания
        /// </summary>
        public DebtingType DebtingType { get; set; }

        /// <summary>
        /// Идентификаторо пользователя в системе
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Сессионный ключ на шлюзе
        /// </summary>
        public string GateSessionKey { get; set; }

        /// <summary>
        /// Время последнего доступа
        /// </summary>
        public DateTime LastAccess { get; set; }
    }
}