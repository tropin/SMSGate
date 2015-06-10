using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Csharper.Common;

namespace Csharper.SenderServiceFacade
{
    /// <summary>
    /// Сессия пользователя на сервисе - фасаде
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
        /// Идентификатор пользователя в системе
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Имя отправителя
        /// </summary>
        [SessionUnique]
        public string SenderName { get; set; }

        /// <summary>
        /// Время последнего доступа
        /// </summary>
        public DateTime LastAccess { get; set; }
    }
}