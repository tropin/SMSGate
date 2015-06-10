using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.SenderService.DAL;
using Csharper.Common;
using Csharper.SenderService.ClientSiteCallback;

namespace Csharper.SenderService
{
    public static class UpdaterExtensions
    {
        private static Dictionary<string, StatusUpdater> _updaters = new Dictionary<string, StatusUpdater>();

        private static object _syncLock = new object();
        
        private static StatusUpdater GetUpdaterInternal(SenderShedullerEntities context)
        {
            string key = context.Connection.ConnectionString;
            lock(_syncLock)
            {
                if (!_updaters.ContainsKey(key))
                {
                    StatusUpdater upd = new StatusUpdater(context);
                    _updaters.Add(key, upd);
                }
                return _updaters[key];
            }
        }

        public static StatusUpdater GetStatusUpdater(this SenderShedullerEntities context)
        {
            return GetUpdaterInternal(context);
        }
    }
    
    public class StatusUpdater
    {
        private SenderShedullerEntities _context;

        private static object _syncLock = new object();

        private const string BACK_REF_BINDING = "SiteGatewayCallback";

        private static System.ServiceModel.CommunicationState[] badStates = new[]
            {
              System.ServiceModel.CommunicationState.Faulted,
              System.ServiceModel.CommunicationState.Closed,
              System.ServiceModel.CommunicationState.Closing              
            };

        private static Dictionary<string, GatewayCallbackClient> _backSitePool = new Dictionary<string, GatewayCallbackClient>();

        private static GatewayCallbackClient GetSiteBackRef(string serviceUrl)
        {
            GatewayCallbackClient client = null;
            bool needReinit = true;
            lock(_syncLock)
            {
                if (_backSitePool.ContainsKey(serviceUrl))
                {
                    client = _backSitePool[serviceUrl];
                    if (!badStates.Contains(client.State))
                        needReinit = false;
                    else
                        _backSitePool.Remove(serviceUrl);
                }
                if (needReinit)
                {
                    client = new GatewayCallbackClient(BACK_REF_BINDING, serviceUrl);
                    _backSitePool.Add(serviceUrl, client);
                }
                return client;
            }
        }

        private SenderShedullerEntities Context
        {
            get
            {
                return _context;
            }
        }

        public StatusUpdater(SenderShedullerEntities context)
        {
            _context = context;
        }

        public void UpdateSMSStatus(Guid? id, string messageId, SMSStatus? status, Guid? providerId, RoaminSMPP.Packet.Pdu.MessageStateType? state)
        {
            UpdateSMSStatus(id, messageId, status, providerId, (short?)state);
        }

        public void UpdateSMSStatus(Guid? id, string messageId, SMSStatus? status, Guid? providerId, RoaminSMPP.Packet.SmppCommandStatus? state)
        {
            UpdateSMSStatus(id, messageId, status, providerId, (short?)state);
        }
        
        private void UpdateSMSStatus(Guid? id, string messageId, SMSStatus? status, Guid? providerId, short? state)
        {
            SMSShortInfo si = _context.UpdateSMSStatus(id, messageId, (short?)status, providerId, (short?)state).FirstOrDefault();
            if (si != null)
            {
                Dictionary<string, string> smsParams = new Dictionary<string, string>();
                var pms = _context.GetSMSParams(si.Id);
                foreach (var par in pms)
                {
                    if (!smsParams.ContainsKey(par.Key))
                    {
                        smsParams.Add(par.Key, par.Value);
                    }
                }
                if (status.HasValue)
                {
                    switch (status.Value)
                    {
                        case SMSStatus.Delivered:
                            GetSiteBackRef(si.CallbackServiceUrl).NotifyDelivered(si.Id, si.ClientId, si.DistributionId, smsParams);
                            break;
                        case SMSStatus.SendError:
                            GetSiteBackRef(si.CallbackServiceUrl).NotifyFailed(si.Id, si.ClientId, si.DistributionId, smsParams);
                            break;
                        case SMSStatus.ValidationError:
                            GetSiteBackRef(si.CallbackServiceUrl).NotifyFailed(si.Id, si.ClientId, si.DistributionId, smsParams);
                            break;
                        case SMSStatus.Cancelled:
                            GetSiteBackRef(si.CallbackServiceUrl).NotifyFailed(si.Id, si.ClientId, si.DistributionId, smsParams);
                            break;
                    }
                }
            }
        }

    }
}
