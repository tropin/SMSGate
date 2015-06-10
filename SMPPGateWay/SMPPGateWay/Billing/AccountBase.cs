using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS.Billing
{
    /// <summary>
    /// Аккаунт в системе
    /// </summary>
    public abstract class AccountBase: IDisposable
    {
        IBillingProvider _provider;
        private string _login;
        private string _sessionKey;

        /// <summary>
        /// Уникальное имя профиля на биллинге
        /// </summary>
        public string Login
        {
            get
            {
                return _login;
            }
        }

        /// <summary>
        /// Количество средств на счету
        /// </summary>
        public decimal Ballance
        {
            get
            {
                return GetBallance(_sessionKey);
            }
        }

        /// <summary>
        /// Сехема оплаты для профиля
        /// </summary>
        public BillingScheme BillingScheme
        {
            get
            {
                return GetBillingScheme(_login);
            }
        }

        /// <summary>
        /// Стоимость сообщения для профиля
        /// </summary>
        public decimal MessageCost
        {
            get
            {
                return GetMessageCost(_login);
            }
        }

        /// <summary>
        /// Стоимость замещения сообщения
        /// </summary>
        public decimal ReplaceCost
        {
            get
            {
                return GetReplaceCost(_login);
            }
        }

        protected abstract decimal GetReplaceCost(string _login);

        protected abstract decimal GetMessageCost(string _login);        

        protected abstract BillingScheme GetBillingScheme(string _login);

        /// <summary>
        /// Получить аккаунт
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        /// <returns>аккаунт</returns>
        protected AccountBase(string login, string password)
        {
            _provider = GetBillingProvider();
            BillingResponce<string> resp = _provider.Login(login, password);
            _sessionKey = GetSessionKey(resp);
            _login = login;
        }
        
        private decimal GetBallance(string _sessionKey)
        {
            BillingResponce<decimal> resp = _provider.GetBallance(_sessionKey);
               return this.GetBallanceValue(resp);
        }

        /// <summary>
        /// Метод анализа ответа конкретного провайдера биллинга и выдача ответа по балансу
        /// </summary>
        /// <param name="resp">ответ биллинга</param>
        /// <returns>значение баланса</returns>
        protected abstract decimal GetBallanceValue(BillingResponce<decimal> resp);

        protected abstract decimal GetIncrementBallanceValue(BillingResponce<decimal> resp);

        protected abstract decimal GetDecrementBallanceValue(BillingResponce<decimal> resp);

        protected abstract bool GetCanPayValue(BillingResponce<bool> resp);

        /// <summary>
        /// Метод анализа ответа конкретного провайдера биллинга и выдача ответа по входу в систему
        /// </summary>
        /// <param name="resp">ответ биллинга</param>
        /// <returns>сессионный ключ</returns>        
        protected abstract string GetSessionKey(BillingResponce<string> resp);

        protected abstract IBillingProvider GetBillingProvider();
        

        /// <summary>
        /// Увеличить баланс на величину
        /// </summary>
        /// <param name="amount">значение на которое осуществляется увеличение</param>
        /// <returns>новое значение баланса</returns>
        public decimal IncrementBallance(decimal amount)
        {
            BillingResponce<decimal> resp = _provider.IncrementBallance(amount,_sessionKey);
            return GetIncrementBallanceValue(resp);
        }
        
        /// <summary>
        /// Уменьшить баланс на величину
        /// </summary>
        /// <param name="amount">значение для снятия</param>
        /// <returns>новое значение баланса</returns>
        public decimal DecrementBallance(decimal amount)
        {
            BillingResponce<decimal> resp = _provider.DecrementBallance(amount,_sessionKey);
            return GetDecrementBallanceValue(resp);
        }

        /// <summary>
        /// Проверить возможность провдения платежа
        /// </summary>
        /// <param name="amount">размер вероятного списания</param>
        /// <returns>да.нет</returns>
        public bool CanPay(decimal amount)
        {
            BillingResponce<bool> resp = _provider.CanPay(amount, _sessionKey);
            return GetCanPayValue(resp);
        }

        #region IDisposable Members

        public void Dispose()
        {
            _provider.Logoff(_sessionKey);
        }

        #endregion
    }
}
