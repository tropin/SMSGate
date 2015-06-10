using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Доступные имена ролей пользователей в системе
    /// </summary>
    public static class RoleNames
    {
        /// <summary>
        /// Центральный мегаадминистратор
        /// </summary>
        public const string ROOT_ADMIN = "RootAdmin";
        /// <summary>
        /// Администратор
        /// </summary>
        public const string ADMIN = "Admin";
        /// <summary>
        /// Менеджер
        /// </summary>
        public const string MANAGER = "Manager";
    }
}
