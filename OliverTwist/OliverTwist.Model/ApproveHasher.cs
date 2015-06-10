using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using Csharper.OliverTwist.Model.Properties;
using System.Text;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Генератор хешей подтверждения для пользователей ресурса
    /// </summary>
    public static class ApproveHasher
    {
        private static HashAlgorithm _internalHasher = SHA256Managed.Create();
        
        /// <summary>
        /// Получить хеш подтверждения для пользователя
        /// </summary>
        /// <param name="user">Пользователь подсиситемы Membership</param>
        /// <returns>Строку хеша</returns>
        public static string GetHash(MembershipUser user)
        {
                string hashTarget = string.Format("{0}_{1}_{2}", user.Email, user.UserName, Settings.Default.SecretKey);
                return Convert.ToBase64String(_internalHasher.ComputeHash(Encoding.UTF8.GetBytes(hashTarget)));
        }

        /// <summary>
        /// Проверка хеша на правильность
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="hash">Предполагаемый хеш</param>
        /// <returns>Истинность утверждения</returns>
        public static bool IsValidHash(MembershipUser user, string hash)
        {
            return GetHash(user) == hash;
        }

    }
}