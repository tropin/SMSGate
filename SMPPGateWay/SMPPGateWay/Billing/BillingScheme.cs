using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS.Billing
{
    /// <summary>
    /// Схема оплаты биллинга
    /// </summary>
    public enum BillingScheme
    {
        /// <summary>
        /// Успешное помещение в очередь
        /// </summary>
        PreSend,
        /// <summary>
        /// Успешная отправка оператору
        /// </summary>
        AfterSend,
        /// <summary>
        /// Успешная доставка получателю
        /// </summary>
        Delivered
    }
}
