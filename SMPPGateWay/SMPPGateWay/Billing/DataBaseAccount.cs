using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS.Billing
{
    public class DataBaseAccount: AccountBase
    {
        public DataBaseAccount(string login, string password) : base(login, password) { }
        
        protected override decimal GetMessageCost(string _login)
        {
            throw new NotImplementedException();
        }

        protected override BillingScheme GetBillingScheme(string _login)
        {
            throw new NotImplementedException();
        }

        protected override decimal GetBallanceValue(BillingResponce<decimal> resp)
        {
            throw new NotImplementedException();
        }

        protected override decimal GetIncrementBallanceValue(BillingResponce<decimal> resp)
        {
            throw new NotImplementedException();
        }

        protected override decimal GetDecrementBallanceValue(BillingResponce<decimal> resp)
        {
            throw new NotImplementedException();
        }

        protected override bool GetCanPayValue(BillingResponce<bool> resp)
        {
            throw new NotImplementedException();
        }

        protected override string GetSessionKey(BillingResponce<string> resp)
        {
            throw new NotImplementedException();
        }

        protected override IBillingProvider GetBillingProvider()
        {
            throw new NotImplementedException();
        }

        protected override decimal GetReplaceCost(string _login)
        {
            throw new NotImplementedException();
        }
    }
}
