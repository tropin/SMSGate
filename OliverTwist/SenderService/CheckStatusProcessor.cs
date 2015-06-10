using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Csharper.SenderService.DAL;
using Csharper.SenderService.Properties;
using RoaminSMPP.Packet;
using System.Collections;
using RoaminSMPP.Utility;
using RoaminSMPP.Packet.Request;
using System.Diagnostics;

namespace Csharper.SenderService
{
    public class CheckStatusProcessor : ProcessorBase<Timer>
    {
        private static Dictionary<string, Guid> _sentMessages = new Dictionary<string, Guid>();
        private static object _syncLock = new object();
        
        public CheckStatusProcessor(TimeSpan interval)
            : base(interval)
        {
            SMPPPool.QueryRequestFinished += new SMPPPool.QueryResult(SMPPPool_QueryRequestFinished);
        }

        private void SMPPPool_QueryRequestFinished(string messageId, Pdu.MessageStateType messageStateType, SmppCommandStatus cStatus)
        {
            lock (_syncLock)
            {
                if (_sentMessages.ContainsKey(messageId) && cStatus == SmppCommandStatus.ESME_ROK)
                {
                    Context.GetStatusUpdater().UpdateSMSStatus(_sentMessages[messageId], messageId, messageStateType.ConvertStatus(), null, messageStateType);
                    _sentMessages.Remove(messageId);
                }
            }
        }
        
        protected override void Process(Timer timer)
        {
            List<SMSQueue> queue = Context.GetSMSToUpdateStatus(Settings.Default.SMSCheckStatusBatchSize, DateTime.Now.Add(-Settings.Default.CheckStatusInterval)).ToList();
            foreach (SMSQueue sms in queue)
            {
                if (!string.IsNullOrEmpty(sms.SMSId))
                {
                    try
                    {
                        RoaminSMPP.SMPPCommunicator connection = SMPPPool.GetConnection(sms.ProviderId.Value, sms.source_addr);
                        SmppQuerySm queryPdu = new SmppQuerySm()
                        {
                            MessageId = sms.SMSId,
                            SourceAddress = sms.source_addr,
                            SourceAddressNpi = connection.NpiType,
                            SourceAddressTon = connection.TonType
                        };
                        connection.SendPdu(queryPdu);
                        lock (_syncLock)
                        {
                            if (!_sentMessages.ContainsKey(sms.SMSId))
                            {
                                _sentMessages.Add(sms.SMSId, sms.Id);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Невозможно отправить сообщение с Id = {0}, ошибка: {1}", sms.Id, ex);
                    }
                }
            }
        }
    }
}
