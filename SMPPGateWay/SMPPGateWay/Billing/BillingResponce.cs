using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS.Billing
{
    public class BillingResponce<T>
    {
        public T Responce { get; private set; }
        public Dictionary<int, string> Errors { get; private set; }

        public BillingResponce(T responce, Dictionary<int,string> errors)
        {
            Responce = responce;
            Errors = errors;
        }
    }
}
