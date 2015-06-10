using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Политика работы с балансом
    /// </summary>
    public static class BallancePolicy
    {
        /// <summary>
        /// Стартовый балланс
        /// </summary>
        public static decimal StartupBallance
        {
            get
            {
                return 10;
            }
        }
        /// <summary>
        /// Стартовая цена сообщения
        /// </summary>
        public static decimal StartupMessageCost
        {
            get
            {
                return 0.5m;
            }
        }

        /// <summary>
        /// Стандартный тип счета
        /// </summary>
        public static AccountType DefaultAccountType
        {
            get
            {
                return AccountType.PostPay; //В отладочных целях
                //return AccountType.PrePay;
            }
        }

        public static DebtingType DefaultDebtingType
        {
            get
            {
                return DebtingType.BySent;
            }
        }
    }
}
