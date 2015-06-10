using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS.Billing
{
    public class DataBaseBillingProvider: IBillingProvider
    {
        #region IBillingProvider Members

        public BillingResponce<string> Login(string login, string password)
        {
            throw new NotImplementedException();
        }

        public BillingResponce<decimal> GetBallance(string sessionKey)
        {
            throw new NotImplementedException();
        }

        public BillingResponce<decimal> IncrementBallance(decimal amount, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public BillingResponce<decimal> DecrementBallance(decimal amount, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public BillingResponce<bool> CanPay(decimal amount, string sessionKey)
        {
            throw new NotImplementedException();
        }

        public AccountBase GetAccount(string login, string password)
        {
            throw new NotImplementedException();
        }

        public void Logoff(string sessionKey)
        {
            throw new NotImplementedException();
        }

        public void Ping(string sessionKey)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
