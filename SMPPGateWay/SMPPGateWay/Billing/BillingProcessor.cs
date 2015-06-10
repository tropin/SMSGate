using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS.Billing
{
    public class BillingProcessor
    {
        private AccountBase _account;

        /// <summary>
        /// Счет, профиль
        /// </summary>
        public AccountBase Account
        {
            get
            {
                return _account;
            }
        }

        public BillingProcessor(AccountBase account)
        {
            _account = account;
        }

        /// <summary>
        /// Сообщение помещено во внутренюю очередь
        /// </summary>
        /// <param name="count">Количество сообщений</param>
        public void MessagesStored(int count)
        {
            if (_account.BillingScheme == BillingScheme.PreSend)
                _account.DecrementBallance(_account.MessageCost * count);
        }

        /// <summary>
        /// Сообщение заменено на сервере
        /// </summary>
        public void MessageReplaced()
        {
            _account.DecrementBallance(_account.ReplaceCost);
        }
        
        /// <summary>
        /// Сообщения отправлены оператору
        /// </summary>
        /// <param name="count"></param>
        public void MessagesSent(int count)
        {
            if (_account.BillingScheme == BillingScheme.AfterSend)
                _account.DecrementBallance(_account.MessageCost * count);
        }

        /// <summary>
        /// Сообщения доставлены адресатам
        /// </summary>
        /// <param name="count"></param>
        public void MessagesDelivered(int count)
        {
            if (_account.BillingScheme == BillingScheme.Delivered)
                _account.DecrementBallance(_account.MessageCost * count);
        }
    }
}
