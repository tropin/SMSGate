using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.SenderService.DAL;
using RoaminSMPP.Utility;
using System.Runtime.Serialization;
using Csharper.SenderService.Properties;
using System.Timers;
using RoaminSMPP.Packet.Request;
using System.Diagnostics;
using RoaminSMPP.Packet;
using RoaminSMPP.Packet.Response;
using Csharper.Common;

namespace Csharper.SenderService
{
    public static class SMPPPool
    {
        private static SenderShedullerEntities _context;
        private static DataContractSerializer _providerConfigurationSerializer = new DataContractSerializer(typeof(ProviderConfiguration));
        private static object _syncLock = new object();

        private static SenderShedullerEntities Context
        {
            get
            {
                if (_context == null)
                    _context = new SenderShedullerEntities();
                return _context;
            }
        }
        
        private static Dictionary<string, ConnectionItem> _connections
            = new Dictionary<string, ConnectionItem>();

        private static string GenerateKey(Guid providerId, string senderName)
        {
            return string.Format("{0}_{1}", providerId, senderName);
        }

        public static RoaminSMPP.SMPPCommunicator GetConnection(Guid providerId, string senderName)
        {
            RoaminSMPP.SMPPCommunicator connection = null;
            string key = GenerateKey(providerId, senderName);
            lock (_syncLock)
            {
                if (_connections.ContainsKey(key))
                {
                    connection = _connections[key].Connection;
                }
                else
                {
                    Providers provider = Context.GetProvider(providerId).FirstOrDefault();
                    if (provider != null)
                    {
                        ProviderConfiguration conf = _providerConfigurationSerializer.Deserialize<ProviderConfiguration>(provider.Configuration);
                        TonNpiPair senderPlan = conf.SourceNumberings.GetNpiTonPair(senderName);
                        if (senderPlan == null)
                            throw new InvalidOperationException(Resources.Error_NumberTypeNotSupported);
                        KeyedTimer<string> connectionTimer = new KeyedTimer<string>(conf.EnqureLinkInterval, key);
                        connectionTimer.Elapsed += new ElapsedEventHandler(connectionTimer_Elapsed);
                        connection = new RoaminSMPP.SMPPCommunicator()
                        {
                            BindType = conf.BindingTypes.FirstOrDefault(),
                            AddressRange = conf.AddressRange,
                            Version = conf.SupportedSMPPVersions.Max(),
                            Host = conf.Host,
                            Port = (short)conf.Port,
                            EnquireLinkInterval = (int)conf.EnqureLinkInterval.TotalSeconds,
                            TonType = senderPlan.Ton,
                            NpiType = senderPlan.Npi,
                            SystemType = conf.SystemType,
                            SystemId = conf.SystemId,
                            Password = conf.Password,
                            SleepTimeAfterSocketFailure = (int)Settings.Default.SleepTimeAfterSocketFailure.TotalSeconds,
                            Username = conf.UserName,
                            ProviderId = provider.Id
                        };
                        connection.OnSubmitMultiResp += new RoaminSMPP.SMPPCommunicator.SubmitMultiRespEventHandler(connection_OnSubmitMultiResp);
                        connection.OnSubmitSmResp += new RoaminSMPP.SMPPCommunicator.SubmitSmRespEventHandler(connection_OnSubmitSmResp);
                        connection.OnQuerySmResp += new RoaminSMPP.SMPPCommunicator.QuerySmRespEventHandler(connection_OnQuerySmResp);
                        connection.OnDeliverSm += new RoaminSMPP.SMPPCommunicator.DeliverSmEventHandler(connection_OnDeliverSm);
                        connection.OnError += new RoaminSMPP.SMPPCommunicator.ErrorEventHandler(connection_OnError);
                        _connections.Add(key, new ConnectionItem
                        {
                            Connection = connection,
                            ConnectionRefreshTimer = connectionTimer,
                            ReconnectAttempts = 0
                        });
                        try
                        {
                            connection.Bind();
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                        }
                        finally
                        {
                            connectionTimer.Enabled = true;
                        }
                    }
                }
            }
            return connection;
        }

        public delegate void SingleSubmitResult(uint sequenceNumber, string messageId, SmppCommandStatus cStatus);
        public delegate void QueryResult(string messageId, RoaminSMPP.Packet.Pdu.MessageStateType messageStateType, SmppCommandStatus cStatus);
        public delegate void MultiSubmitResult(uint sequenceNumber, string messageId, SmppCommandStatus cStatus, UnsuccessAddress[] unsuccessAddress);
        public delegate void DeliverResult(string messageId,Guid providerId, uint sequenceNumber, Pdu.MessageStateType messageStateType, string networkError);

        public static event QueryResult QueryRequestFinished;
        public static event SingleSubmitResult SingleSubmitFinished;
        public static event MultiSubmitResult MultiSubmitFinished;
        public static event DeliverResult DeliveryRecieptRecieved;

        static void connection_OnQuerySmResp(object source, RoaminSMPP.EventObjects.QuerySmRespEventArgs e)
        {
            if (QueryRequestFinished != null)
                QueryRequestFinished(e.QuerySmRespPdu.MessageId, e.QuerySmRespPdu.MessageStatus, (SmppCommandStatus)e.ResponsePdu.CommandStatus);
        }

        static void connection_OnDeliverSm(object source, RoaminSMPP.EventObjects.DeliverSmEventArgs e)
        {
            string networkMessage = string.Empty;
            try
            {
                networkMessage = e.DeliverSmPdu.NetworkErrorCode;
            }
            catch
            {
                networkMessage = e.DeliverSmPdu.ShortMessage;
            }
            RoaminSMPP.SMPPCommunicator connection = source as RoaminSMPP.SMPPCommunicator;
            if (connection != null)
            {
            if (DeliveryRecieptRecieved != null)
                DeliveryRecieptRecieved(e.DeliverSmPdu.ReceiptedMessageId, connection.ProviderId, e.DeliverSmPdu.SequenceNumber, e.DeliverSmPdu.MessageState, networkMessage);
                SmppDeliverSmResp resp = new SmppDeliverSmResp()
                {
                    CommandStatus = (uint)SmppCommandStatus.ESME_ROK,
                    SequenceNumber = e.DeliverSmPdu.SequenceNumber
                };
                connection.SendPdu(resp);
            }
        }

        static void connection_OnSubmitSmResp(object source, RoaminSMPP.EventObjects.SubmitSmRespEventArgs e)
        {
            if (SingleSubmitFinished != null)
                SingleSubmitFinished(e.SubmitSmPdu.SequenceNumber, e.SubmitSmPdu.MessageId, (SmppCommandStatus)e.ResponsePdu.CommandStatus);
        }

        static void connection_OnSubmitMultiResp(object source, RoaminSMPP.EventObjects.SubmitMultiRespEventArgs e)
        {
            if (MultiSubmitFinished != null)
                MultiSubmitFinished(e.SubmitMultiRespPdu.SequenceNumber, e.SubmitMultiRespPdu.MessageId, (SmppCommandStatus)e.ResponsePdu.CommandStatus, e.SubmitMultiRespPdu.UnsuccessfulAddresses);
        }

        static void connection_OnError(object source, RoaminSMPP.EventObjects.CommonErrorEventArgs e)
        {
            Trace.TraceError("Ошибка при работе с соединением: {0}", e.ThrownException.ToString());
        }

        static void connectionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (sender is KeyedTimer<string>)
            {
                KeyedTimer<string> timer = sender as KeyedTimer<string>;
                lock (_syncLock)
                {
                    if (_connections.ContainsKey(timer.Key))
                    {
                        ConnectionItem item = _connections[timer.Key];
                        if (!item.IsRunning)
                        {
                            try
                            {
                                RoaminSMPP.SMPPCommunicator conn = _connections[timer.Key].Connection;
                                if (conn.IsBinded)
                                {
                                    conn.SendPdu(new SmppEnquireLink());
                                }
                                else
                                {
                                    conn.Bind();
                                }
                            }
                            catch(Exception ex)
                            {
                                Trace.TraceWarning("Ошибка соединения с поставщиком: {0}", ex);
                                item.ReconnectAttempts++;
                                if (item.ReconnectAttempts > Settings.Default.MaxReconnectAttempts)
                                {
                                    item.ConnectionRefreshTimer.Enabled = false;
                                    item.ConnectionRefreshTimer.Elapsed -= connectionTimer_Elapsed;
                                    _connections.Remove(timer.Key);
                                    item.ConnectionRefreshTimer.Dispose();
                                    item.Connection.Dispose();
                                }
                            }
                            finally
                            {
                                item.IsRunning = false;
                            }
                        }
                    }
                }
            }
        }
        
    }
}
