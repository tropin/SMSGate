using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoaminSMPP;
using System.Diagnostics;
using Csharper.SMS.Billing;
using Csharper.SMS.Storage;
using RoaminSMPP.EventObjects;
using RoaminSMPP.Packet.Response;
using RoaminSMPP.Packet;
using Csharper.SMS.Services;

namespace Csharper.SMS.SMSC
{
    /// <summary>
    /// Обрабатывает команды от входящих клиентов, реализуя поведение протокола
    /// </summary>
    public class ServerProcessor
    {
        private const string CONNECTION_COUNTER_NAME = "smpp_act_conn";
        
        private SMPPCommunicator communicator = null;
        private ISMSStorageProvider storage = null;
        private IBillingProvider billingprov = null;
        private BillingProcessor billing = null;
        
        public ServerProcessor(AsyncSocketClient client, ISMSStorageProvider storagerProvider, IBillingProvider provider)
        {
            communicator = new SMPPCommunicator();
            storage = storagerProvider;
            communicator.Disposed += new EventHandler(communicator_Disposed);
            communicator.OnAlert += new SMPPCommunicator.AlertEventHandler(communicator_OnAlert);
            communicator.OnBind += new SMPPCommunicator.BindEventHandler(communicator_OnBind);
            communicator.OnCancelSm += new SMPPCommunicator.CancelSmEventHandler(communicator_OnCancelSm);
            communicator.OnClose += new SMPPCommunicator.ClosingEventHandler(communicator_OnClose);
            communicator.OnDataSm += new SMPPCommunicator.DataSmEventHandler(communicator_OnDataSm);
            communicator.OnDeliverSm += new SMPPCommunicator.DeliverSmEventHandler(communicator_OnDeliverSm);
            communicator.OnDeliverSmResp += new SMPPCommunicator.DeliverSmRespEventHandler(communicator_OnDeliverSmResp);
            communicator.OnEnquireLink += new SMPPCommunicator.EnquireLinkEventHandler(communicator_OnEnquireLink);
            communicator.OnError += new SMPPCommunicator.ErrorEventHandler(communicator_OnError);
            communicator.OnQuerySm += new SMPPCommunicator.QuerySmEventHandler(communicator_OnQuerySm);            
            communicator.OnReplaceSm += new SMPPCommunicator.ReplaceSmEventHandler(communicator_OnReplaceSm);
            communicator.OnSubmitMulti += new SMPPCommunicator.SubmitMultiEventHandler(communicator_OnSubmitMulti);
            communicator.OnSubmitSm += new SMPPCommunicator.SubmitSmEventHandler(communicator_OnSubmitSm);
            communicator.OnUnbind += new SMPPCommunicator.UnbindEventHandler(communicator_OnUnbind);
            communicator.ProcessServerClient(client);
        }

        /// <summary>
        /// Клиент пробует подключиться
        /// </summary>
        void communicator_OnBind(object source, BindEventArgs e)
        {
            SmppBindResp pdu = new SmppBindResp();
            pdu.SequenceNumber = e.BindPdu.SequenceNumber;

            if (!communicator.IsBinded)
            {
                //Биндинг соединения
                AccountBase account = null;
                try
                {
                    account = billingprov.GetAccount(e.BindPdu.SystemId, e.BindPdu.Password);
                    billing = new BillingProcessor(account);
                    pdu.CommandStatus = (uint)SmppCommandStatus.ESME_ROK;
                    e.IsBindSucessfull = true;
                    LoggerService.Logger.TraceEvent(TraceEventType.Information, LoggingCatoegory.Protocol.IntValue(), string.Format("Client {0} binded", e.BindPdu.SystemId));
                    PerformanceCountersService.GetCounter(CONNECTION_COUNTER_NAME).Increment();
                }
                catch (Exception ex)
                {
                    LoggerService.Logger.TraceEvent(TraceEventType.Error, LoggingCatoegory.Protocol.IntValue(), string.Format("Bind failed in fact of account for user {0} cannot be get. Error {1}", e.BindPdu.SystemId, ex.ToString()));
                    pdu.CommandStatus = (uint)SmppCommandStatus.ESME_RBINDFAIL;
                    e.IsBindSucessfull = false;
                }
            }
            else
            {
                pdu.CommandStatus = (uint)SmppCommandStatus.ESME_RALYBND;
                e.IsBindSucessfull = false;
            }
            communicator.SendPdu(pdu);
        }
        
        /// <summary>
        /// Комманда отсоединения
        /// </summary>
        void communicator_OnUnbind(object source, UnbindEventArgs e)
        {
            //Отсоединяемся
            if (communicator.IsBinded && billing != null)
            {
                LoggerService.Logger.TraceEvent(TraceEventType.Information, LoggingCatoegory.Protocol.IntValue(), string.Format("Client {0} unbinded", billing.Account.Login));
                billing.Account.Dispose();
                billing = null;
                PerformanceCountersService.GetCounter(CONNECTION_COUNTER_NAME).Decrement();
            }
            SmppUnbindResp pdu = new SmppUnbindResp();
            pdu.SequenceNumber = e.UnbindPdu.SequenceNumber;
            pdu.CommandStatus = (uint)SmppCommandStatus.ESME_ROK;
        }

        /// <summary>
        /// Выставление в очередь одиночного сообщения
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void communicator_OnSubmitSm(object source, SubmitSmEventArgs e)
        {   
            SmppSubmitSmResp pdu;
            //Посылаем сообщение
            if (communicator.IsBinded)
            {
                //TODO: Здесь еще правильно осуществить валидацию
                //pdu.CommandStatus = (uint)SmppCommandStatus.ESME_RSUBMITFAIL
                if (billing == null)
                    throw new InvalidOperationException("Cannot submit messages with billing connection not initialized!");
                pdu = storage.StoreMessage(e.SubmitSmPdu);
                if (pdu.CommandStatus == (int)SmppCommandStatus.ESME_ROK)
                {
                    billing.MessagesStored(1);
                    LoggerService.Logger.TraceEvent(TraceEventType.Information, LoggingCatoegory.Protocol.IntValue(), string.Format("Message by client {0} stored by sumit_sm to number {1}", billing.Account.Login, e.SubmitSmPdu.DestinationAddress));
                }
            }
            else
            {
                pdu = new SmppSubmitSmResp();
                pdu.SequenceNumber = e.SubmitSmPdu.SequenceNumber;
                pdu.MessageId = System.Guid.NewGuid().ToString().Substring(0, 10);
                pdu.CommandStatus = (uint)SmppCommandStatus.ESME_RINVBNDSTS;
            }
            communicator.SendPdu(pdu);
        }


        void communicator_OnSubmitMulti(object source, SubmitMultiEventArgs e)
        {
           //Множественная посылка
            SmppSubmitMultiResp pdu;
            //Посылаем сообщение
            if (communicator.IsBinded)
            {
                if (billing == null)
                    throw new InvalidOperationException("Cannot submit messages with billing not initialized!");
                //TODO: Здесь еще правильно осуществить валидацию
                //pdu.CommandStatus = (uint)SmppCommandStatus.ESME_RSUBMITFAIL
                pdu = storage.StoreMessage(e.SubmitMultiPdu);
                if (pdu.CommandStatus == (int)SmppCommandStatus.ESME_ROK && e.SubmitMultiPdu.DestinationAddresses != null && e.SubmitMultiPdu.DestinationAddresses.Length > 0)
                {
                    billing.MessagesStored(e.SubmitMultiPdu.DestinationAddresses.Length);
                    LoggerService.Logger.TraceEvent(TraceEventType.Information, LoggingCatoegory.Protocol.IntValue(), string.Format("Message by client {0} stored by sumit_sm_multi to numbers {1}", billing.Account.Login, string.Join(", ", e.SubmitMultiPdu.DestinationAddresses.Select(addr => addr.DestAddress))));
                }
            }
            else
            {
                pdu = new SmppSubmitMultiResp();
                pdu.SequenceNumber = e.SubmitMultiPdu.SequenceNumber;
                pdu.MessageId = System.Guid.NewGuid().ToString().Substring(0, 10);
                pdu.CommandStatus = (uint)SmppCommandStatus.ESME_RINVBNDSTS;
            }
            communicator.SendPdu(pdu);
        }


        void communicator_OnReplaceSm(object source, ReplaceSmEventArgs e)
        {
            SmppReplaceSmResp pdu;
            //Замещение сохраненного сообщения
            if (communicator.IsBinded)
            {
                if (billing == null)
                    throw new InvalidOperationException("Cannot submit messages with billing not initialized!");
                //TODO: Здесь еще правильно осуществить валидацию
                //pdu.CommandStatus = (uint)SmppCommandStatus.ESME_RSUBMITFAIL
                pdu = storage.ReplaceMessage(e.ReplaceSmPdu);
                if (pdu.CommandStatus == (int)SmppCommandStatus.ESME_ROK)
                {
                    billing.MessageReplaced();
                    LoggerService.Logger.TraceEvent(TraceEventType.Information, LoggingCatoegory.Protocol.IntValue(), string.Format("Message {1} by client {0} replaced by replace_sm", billing.Account.Login, e.ReplaceSmPdu.MessageId));
                }
            }
            else
            {
                pdu = new SmppReplaceSmResp();
                pdu.SequenceNumber = e.ReplaceSmPdu.SequenceNumber;
                pdu.CommandStatus = (uint)SmppCommandStatus.ESME_RINVBNDSTS;
            }
            communicator.SendPdu(pdu);
        }

        void communicator_OnQuerySm(object source, QuerySmEventArgs e)
        {
            //Запрос состояния сохраненного сообщения
        }

        void communicator_OnError(object source, CommonErrorEventArgs e)
        {
            //Ошибка работы комуникатора
            PerformanceCountersService.GetCounter(CONNECTION_COUNTER_NAME).Decrement();
        }

        void communicator_OnEnquireLink(object source, EnquireLinkEventArgs e)
        {
            //Надо записать, что был пинг
        }

        void communicator_OnDeliverSmResp(object source, DeliverSmRespEventArgs e)
        {
            //Ответ, что сохранено в телефон
        }

        void communicator_OnDeliverSm(object source, DeliverSmEventArgs e)
        {
            //Поместить в очередь полученных
        }

        void communicator_OnDataSm(object source, DataSmEventArgs e)
        {
            //Пересылка датаграммы
        }

        void communicator_OnClose(object source, EventArgs e)
        {
            //Записать, что сокет закрылся
            PerformanceCountersService.GetCounter(CONNECTION_COUNTER_NAME).Decrement();
        }

        void communicator_OnCancelSm(object source, CancelSmEventArgs e)
        {
            //Отмена записанного, но не отмененного сообщения
        }

        void communicator_OnAlert(object source, AlertEventArgs e)
        {
            //Предупреждние
        }

        void communicator_Disposed(object sender, EventArgs e)
        {
            //Сокет уничтожен
            PerformanceCountersService.GetCounter(CONNECTION_COUNTER_NAME).Decrement();
        }
    }
}
