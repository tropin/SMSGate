using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoaminSMPP.Packet.Request;
using RoaminSMPP.Packet.Response;

namespace Csharper.SMS.Storage
{
    /// <summary>
    /// Провайдер хранения и обработки сообщений
    /// </summary>
    public interface  ISMSStorageProvider
    {
        /// <summary>
        /// Сохранить сообщение на сервере
        /// </summary>
        /// <param name="packet">Пакет отправки</param>
        /// <returns>Результат сохранения</returns>
        SmppSubmitSmResp StoreMessage(SmppSubmitSm packet);
        /// <summary>
        /// Сохранить сообщение на сервере
        /// </summary>
        /// <param name="packet">Пакет отправки</param>
        /// <returns>Результат сохранения</returns>
        SmppDataSmResp StoreMessage(SmppDataSm packet);
        /// <summary>
        /// Сохранить сообщение на сервере с несколькими получателями
        /// </summary>
        /// <param name="packet">Пакет отправки</param>
        /// <returns>Результат сохранения</returns>
        SmppSubmitMultiResp StoreMessage(SmppSubmitMulti packet);
        /// <summary>
        /// Заменить сообщение на сервере
        /// </summary>
        /// <param name="packet">Пакет замещения</param>
        /// <returns>Результат замещения</returns>
        SmppReplaceSmResp ReplaceMessage(SmppReplaceSm packet);
        /// <summary>
        /// Опросить статус сообщения на сервере
        /// </summary>
        /// <param name="packet">Пакет замещения</param>
        /// <returns>Результат замещения</returns>        
        SmppQuerySmResp QueryMessage(SmppQuerySm packet);

        /// <summary>
        /// Сохранить сообщение в список входящих
        /// </summary>
        /// <param name="packet">Пакет сообщения</param>
        /// <returns>Результат сохранения</returns>
        SmppDeliverSmResp StoreIncomingMessage(SmppDeliverSm packet);
 
    }
}

