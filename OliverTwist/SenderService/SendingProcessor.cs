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
using Csharper.Common;

namespace Csharper.SenderService
{
    public class SendingProcessor: ProcessorBase<Timer>
    {
        private static Dictionary<uint, Guid> _sentMessages = new Dictionary<uint, Guid>();
        private static object _syncLock = new object();

        public SendingProcessor(TimeSpan interval): base(interval)
        {
            SMPPPool.MultiSubmitFinished += new SMPPPool.MultiSubmitResult(SMPPPool_MultiSubmitFinished);
            SMPPPool.SingleSubmitFinished += new SMPPPool.SingleSubmitResult(SMPPPool_SingleSubmitFinished);
            SMPPPool.DeliveryRecieptRecieved += new SMPPPool.DeliverResult(SMPPPool_DeliveryRecieptRecieved);
        }

        private void SMPPPool_DeliveryRecieptRecieved(string messageId, Guid providerId, uint sequenceNumber, Pdu.MessageStateType messageStateType, string networkError)
        {
            lock (_syncLock)
            {
                if (messageStateType == Pdu.MessageStateType.Delivered)
                {
                    Context.GetStatusUpdater().UpdateSMSStatus(null, messageId, SMSStatus.Delivered, providerId, messageStateType);
                }
                else
                {
                    Context.GetStatusUpdater().UpdateSMSStatus(null, messageId, SMSStatus.SendError, providerId, messageStateType);
                }
            }
        }

        private void SMPPPool_SingleSubmitFinished(uint sequenceNumber, string messageId, SmppCommandStatus cStatus)
        {
            lock (_syncLock)
            {
                if (_sentMessages.ContainsKey(sequenceNumber))
                {
                    if (cStatus == SmppCommandStatus.ESME_ROK)
                    {
                        Context.GetStatusUpdater().UpdateSMSStatus(_sentMessages[sequenceNumber], messageId, SMSStatus.Send, null, RoaminSMPP.Packet.Pdu.MessageStateType.Accepted);
                    }
                    else
                    {
                        Context.GetStatusUpdater().UpdateSMSStatus(_sentMessages[sequenceNumber], messageId, SMSStatus.ValidationError, null, cStatus);
                    }
                    _sentMessages.Remove(sequenceNumber);
                }
            }
        }

        private void SMPPPool_MultiSubmitFinished(uint sequenceNumber, string messageId, SmppCommandStatus cStatus, UnsuccessAddress[] unsuccessAddress)
        {
            lock (_syncLock)
            {
                if (_sentMessages.ContainsKey(sequenceNumber))
                {
                    if (cStatus == SmppCommandStatus.ESME_ROK)
                    {
                        Context.GetStatusUpdater().UpdateSMSStatus(_sentMessages[sequenceNumber], messageId, SMSStatus.Send, null, RoaminSMPP.Packet.Pdu.MessageStateType.Accepted);
                    }
                    else
                    {
                        Context.GetStatusUpdater().UpdateSMSStatus(_sentMessages[sequenceNumber], messageId, SMSStatus.SendError, null, cStatus);
                    }
                    _sentMessages.Remove(sequenceNumber);
                }
            }
        }
        
        protected override void Process(Timer timer)
        {
            List<SMSQueue> queue = Context.GetSMSToSend(Settings.Default.SMSSendBatchSize).ToList();
            foreach (SMSQueue sms in queue)
            {
                try
                {
                    Trace.TraceInformation("Отправка сообщения {0}", sms.Id);
                    DestinationAddress[] dests = sms.DestinationMap.Select
                            (
                                item =>
                                    item.IsDistributionList ?
                                    new DestinationAddress(item.destination_addr) :
                                    new DestinationAddress(
                                        (Pdu.TonType)item.dest_addr_ton,
                                        (Pdu.NpiType)item.dest_addr_npi,
                                        item.destination_addr)
                            ).ToArray();
                    MessageLcd2 sendPdu;
                    if (sms.number_of_dests > 1)
                    {
                        RoaminSMPP.Packet.Request.SmppSubmitMulti multi = new RoaminSMPP.Packet.Request.SmppSubmitMulti();
                        multi.DestinationAddresses = dests;
                        sendPdu = multi;
                    }
                    else
                    {
                        DestinationAddress dest = dests[0];
                        RoaminSMPP.Packet.Request.SmppSubmitSm single = new RoaminSMPP.Packet.Request.SmppSubmitSm()
                        {
                            DestinationAddress = dest.DestAddress,
                            DestinationAddressNpi = dest.DestinationAddressNpi,
                            DestinationAddressTon = dest.DestinationAddressTon
                        };
                        sendPdu = single;
                    }
                    Hashtable tlvTable = new Hashtable();
                    sms.SMSTLV.ToList().ForEach(item => tlvTable.Add(Convert.ToUInt16(item.Tag), Convert.FromBase64String(item.Value)));
                    sendPdu.TlvTable.tlvTable = tlvTable;
                    sendPdu.SequenceNumber = (uint)sms.sequence_number;
                    sendPdu.ServiceType = sms.service_type;
                    sendPdu.SourceAddress = sms.source_addr;
                    sendPdu.EsmClass = (byte)sms.esm_class;
                    sendPdu.ProtocolId = (Pdu.SmppVersionType)sms.protocol_id;
                    sendPdu.PriorityFlag = (Pdu.PriorityType)sms.priority_flag;
                    sendPdu.ScheduleDeliveryTime = sms.schedule_delivery_time;
                    sendPdu.ValidityPeriod = sms.validity_period;
                    sendPdu.RegisteredDelivery = (Pdu.RegisteredDeliveryType)sms.registered_delivery;
                    sendPdu.ReplaceIfPresentFlag = sms.replace_if_present_flag >= 1 ? true : false;
                    sendPdu.DataCoding = (Pdu.DataCodingType)sms.data_coding;
                    sendPdu.SmDefaultMessageId = (byte)(sms.sm_default_msg_id ?? 0);
                    sendPdu.ShortMessage = sms.short_message.GetBytesFromHex();
                    RoaminSMPP.SMPPCommunicator connection = SMPPPool.GetConnection(sms.ProviderId.Value, sms.source_addr);
                    sendPdu.SourceAddressNpi = connection.NpiType;
                    sendPdu.SourceAddressTon = connection.TonType;
                    sendPdu.RegisteredDelivery = (Pdu.RegisteredDeliveryType)sms.registered_delivery;
                    connection.SendPdu(sendPdu);
                    Context.GetStatusUpdater().UpdateSMSStatus(sms.Id, null, SMSStatus.Send, null, RoaminSMPP.Packet.Pdu.MessageStateType.Accepted);
                    lock (_syncLock)
                    {
                        _sentMessages.Add(sendPdu.SequenceNumber, sms.Id);
                    }
                }
                catch (Exception ex)
                {
                    //TODO: Реализовать логику RetryCount
                    Trace.TraceWarning("Невозможно отправить сообщение с Id = {0}, ошибка: {1}", sms.Id, ex);
                }
            }
        }
    }
}
