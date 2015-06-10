using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Repo;

namespace Csharper.OliverTwist.Model.Billing
{
    /// <summary>
    /// Обработчик операций со счетами и балансом
    /// </summary>
    public class BillingProcessor
    {
        private static DBDataContext _dataContext = null;

        private static object _syncLock = new object();

        /// <summary>
        /// Контекст работы с базой данных
        /// </summary>
        private static DBDataContext DataContext
        {
            get
            {
                lock (_syncLock)
                {
                    if (_dataContext == null)
                        _dataContext = new DBDataContext();
                    if (_dataContext.Connection.State == System.Data.ConnectionState.Closed)
                        _dataContext.Connection.Open();
                    return _dataContext;
                }
            }
        }

        
        public static bool SMSBeginSend(long clientId, long? count = null, long? distibutionId = null, string extDistributionId = null)
        {
            /*Здесь для разных типов счетов и оплаты разное поведение
            для предоплатников - проверяются достуные средства
            блокирование средств, если нет рассылки (при создании рассылки, блокировка берется на всю рассылку)
             */
            var res = DataContext.LockSMS(clientId, count, distibutionId, extDistributionId);
            return (int)res.ReturnValue == 1;
        }

        public static bool CommintSMSSend(long clientId, long? count = null, long? distributionId = null, string extDistributionId = null)
        {
            /*
              Метод вызывается после отправки или после доставки в зависимости
              от принятого для клиента способа оплаты
              Снятие блокироки (возврат средств) или вычет средств со счета
            */
            var res = DataContext.FinalizeSMSLock(clientId, count, distributionId, extDistributionId, true);
            return res == 1;
        }

        public static bool ResetSMSSend(long clientId, long? count = null, long? distributionId = null, string extDistributionId = null)
        {
            var res = DataContext.FinalizeSMSLock(clientId, count, distributionId, extDistributionId, false);
            return res == 1;
        }

    }
}
