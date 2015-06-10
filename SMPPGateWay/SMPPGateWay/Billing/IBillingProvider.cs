using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS.Billing
{
    /// <summary>
    /// Поставщик информации о счетах пользоваетелей
    /// </summary>
    public interface IBillingProvider
    {
        /// <summary>
        /// Вход в систему учета пользователей
        /// </summary>
        /// <param name="login">Позывной</param>
        /// <param name="password">Пароль</param>
        /// <returns>сессионный ключ</returns>
        BillingResponce<string> Login(string login, string password);
        /// <summary>
        /// Получение текущего баланса
        /// </summary>
        /// <param name="sessionKey">сессионный ключ</param>
        /// <returns>текущее значение баланса</returns>
        BillingResponce<decimal> GetBallance(string sessionKey);
        /// <summary>
        /// Увеличение значения баланса на значение
        /// </summary>
        /// <param name="amount">значение для увеличения</param>
        /// <param name="sessionKey">сессионный ключ</param>
        /// <returns>новое значение баланса</returns>
        BillingResponce<decimal> IncrementBallance(decimal amount, string sessionKey);
        /// <summary>
        /// Уменьшение значения баланса на значение
        /// </summary>
        /// <param name="amount">значение для уменьшения</param>
        /// <param name="sessionKey">сессионный ключ</param>
        /// <returns>новое значение баланса</returns>
        BillingResponce<decimal> DecrementBallance(decimal amount, string sessionKey);
        /// <summary>
        /// Функция проверки, что пользователь может осуществить платеж
        /// </summary>
        /// <param name="amount">Размер платежа</param>
        /// <returns>флаг возможности</returns>
        BillingResponce<bool> CanPay(decimal amount, string sessionKey);

        /// <summary>
        /// Получить аккаунт
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        /// <returns>аккаунт</returns>
        AccountBase GetAccount(string login, string password);

        /// <summary>
        /// Выход из подсистемы биллинга
        /// </summary>
        /// <param name="sessionKey">Сессионный ключ</param>
        void Logoff(string sessionKey);

        /// <summary>
        /// Метод поддержания сессии на стороне биллинга
        /// </summary>
        /// <param name="sessionKey">Сессионный ключ</param>
        void Ping(string sessionKey);

    }
}
