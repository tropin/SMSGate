using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Security.Cryptography;
using Csharper.Common;
using Csharper.OliverTwist.Properties;
using System.Web.Security;
using Csharper.OliverTwist.GateService;
using Csharper.OliverTwist.Repo;
using System.Diagnostics;
using Csharper.OliverTwist.Model.Billing;
using Csharper.OliverTwist.Model;

namespace Csharper.OliverTwist.Services
{
    public class ADEService : IADEService
    {
        public const string USER_ID = "UserId";
        public const string EXTERNAL = "Ext";

        private static SessionManager<ServiceSession> _sessionManager =
            new SessionManager<ServiceSession>(Settings.Default.SessionsTTL);

        private static ADEServiceClient _gateService = new ADEServiceClient(); 
        
        #region IADEService Members

        public string Login(string login, string password, string senderName)
        {
            string result = string.Empty;
            if (Membership.ValidateUser(login, password))
            {
                MembershipUser mu = Membership.GetUser(login);
                ClientModel client = ClientRepo.GetClientProjected((Guid)mu.ProviderUserKey);

                string gateSessionKey = _gateService.Login(Properties.Settings.Default.GateUserName, Properties.Settings.Default.GatePassword, senderName);
                if (!string.IsNullOrEmpty(gateSessionKey) && client!=null && client.Id!=null)
                {
                    ServiceSession draft = new ServiceSession()
                    {
                        GateSessionKey = gateSessionKey,
                        LastAccess = DateTime.Now,
                        Login = login,
                        SenderName = senderName,
                        ClientId = client.Id.Value.ToString(),
                        UserId = ((Guid)mu.ProviderUserKey).ToString(),
                        DebtingType = client.DebtingType
                    };
                    draft = _sessionManager.GetSession(draft);
                    if (draft != null)
                    {
                        result = draft.SessionKey;
                    }
                }
            }
            return result;
        }

        public List<SMSSeriesId> SendSms(string sessionKey, List<string> addresses, string message, string distibutionId = null, string messageId = null, bool transliterate = false, DateTime? deliveryTime = null, TimeSpan? validalityPeriod = null)
        {
            List<SMSSeriesId> ids = new List<SMSSeriesId>();
            ServiceSession session = _sessionManager.GetSession(sessionKey);

            if (session != null)
            {
                Dictionary<string, string> custom = new Dictionary<string, string>();
                custom.Add(USER_ID, session.UserId);
                custom.Add(EXTERNAL, true.ToString());
                bool isSent = false;
                long clientId = long.Parse(session.ClientId);
                if (BillingProcessor.SMSBeginSend(clientId, extDistributionId: distibutionId))
                {
                    try
                    {
                        try
                        {
                            ids = _gateService.SendSms(
                                session.GateSessionKey,
                                addresses,
                                message,
                                session.ClientId,
                                distibutionId,
                                messageId,
                                transliterate,
                                deliveryTime,
                                validalityPeriod,
                                custom);
                        }
                        catch
                        {
                            session.GateSessionKey = _gateService.Login(Settings.Default.GateUserName, Settings.Default.GatePassword, session.SenderName);
                            ids = _gateService.SendSms(
                                session.GateSessionKey,
                                addresses,
                                message,
                                session.ClientId,
                                distibutionId,
                                messageId,
                                transliterate,
                                deliveryTime,
                                validalityPeriod,
                                custom);
                        }
                        isSent = true;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Ошибка отправки на шлюз {0}", ex);
                        isSent = false;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Блокировка средств на вашем счету завершилась неудачей, отправка невозможна, проверьте ваш баланс");
                }

                if (isSent && session.DebtingType == DebtingType.BySent)
                {
                    BillingProcessor.CommintSMSSend(clientId, extDistributionId: distibutionId);
                }
                else
                {
                    BillingProcessor.ResetSMSSend(clientId, extDistributionId: distibutionId);
                }
                return ids;
            }
            else
                throw new Exception(Resources.Error_SessionIncorrect);
        }

        public List<SMSCheckItem> CheckSMSByDistribution(string sessionKey, string distributionId, long? rowsPerPage = null, long? pageNumber = null)
        {
            List<SMSCheckItem> result = new List<SMSCheckItem>();
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
            {
                Dictionary<string, string> custom = new Dictionary<string, string>();
                custom.Add(USER_ID, session.UserId);
                custom.Add(EXTERNAL, true.ToString());
                try
                {
                    result = _gateService.CheckSMSStatuses(session.GateSessionKey, session.ClientId, distributionId, null, null, rowsPerPage, pageNumber, custom);
                }
                catch
                {
                    session.GateSessionKey = _gateService.Login(Settings.Default.GateUserName, Settings.Default.GatePassword, session.SenderName);
                    result = _gateService.CheckSMSStatuses(session.GateSessionKey, session.ClientId, distributionId, null, null, rowsPerPage, pageNumber, custom);
                }
                return result;
            }
            else
                throw new Exception(Resources.Error_SessionIncorrect); 
        }

        public List<SMSCheckItem> CheckSMSByMessageId(string sessionKey, string messageId, long? rowsPerPage = null, long? pageNumber = null)
        {
            List<SMSCheckItem> result = new List<SMSCheckItem>();
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
            {
                try
                {
                    result = _gateService.CheckSMSByMessageId(session.GateSessionKey, messageId, rowsPerPage, pageNumber, session.ClientId);
                }
                catch
                {
                    session.GateSessionKey = _gateService.Login(Settings.Default.GateUserName, Settings.Default.GatePassword, session.SenderName);
                    result = _gateService.CheckSMSByMessageId(session.GateSessionKey, messageId, rowsPerPage, pageNumber, session.ClientId);
                }
                return result;
            }
            else
                throw new Exception(Resources.Error_SessionIncorrect); 
        }

        public SMSCheckItem CheckSMSById(string sessionKey, Guid Id)
        {
            SMSCheckItem result;
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
            {
                try
                {
                    result = _gateService.CheckSMSById(session.GateSessionKey, Id, session.ClientId);
                }
                catch
                {
                    session.GateSessionKey = _gateService.Login(Settings.Default.GateUserName, Settings.Default.GatePassword, session.SenderName);
                    result = _gateService.CheckSMSById(session.GateSessionKey, Id, session.ClientId);
                }
                return result;
            }
            else
                throw new Exception(Resources.Error_SessionIncorrect); 
        }

        public SMSCheckItem CheckSMS(string login, string password, Guid id)
        {
            SMSCheckItem result = null;

            if (Membership.ValidateUser(login, password))
            {
                MembershipUser mu = Membership.GetUser(login);
                long? cid = ClientRepo.GetClientProjected((Guid)mu.ProviderUserKey).Id;
                if (cid != null)
                {
                    result = _gateService.CheckSMS(Settings.Default.GateUserName, Settings.Default.GatePassword, id, cid.Value.ToString());
                }
            }
            else
                throw new Exception(Resources.Error_LoginFailed);
            return result;           
        }

        public void LogOff(string sessionKey)
        {
            _sessionManager.KillSession(sessionKey);
        }

        #endregion
    }
}
