using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Security.Cryptography;
using Csharper.SenderService;
using Csharper.SenderServiceFacade.Properties;
using Csharper.Common;

namespace Csharper.SenderServiceFacade
{
    public class ADEService : IADEService
    {
        private static SessionManager<ServiceSession> _sessionManager = 
            new SessionManager<ServiceSession>(Properties.Settings.Default.SessionsTTL);

        private Service _senderService;

        private Service SenderService
        {
            get
            {
                if (_senderService == null)
                    _senderService = new Service();
                return _senderService;
            }
        }

        #region IADEService Members

        public string Login(string login, string password, string senderName)
        {
            string result = string.Empty;
            bool canLogIn = false;
            Guid? user = SenderService.Login(login, password);
            if (user != null)
            {
                if (SenderService.HaveSenderName(user.Value, senderName))
                {
                    canLogIn = true;
                }
            }
            if (canLogIn)
            {
                ServiceSession session = _sessionManager.GetSession(
                        new ServiceSession()
                        {
                            UserId = user.Value,
                            Login = login,
                            SenderName = senderName
                        }
                    );
                if (session != null)
                    result = session.SessionKey;
            }
            return result;
        }

        public List<SMSSeriesId> SendSms(string sessionKey, List<string> addresses, string message, string clientId = null, string distibutionId = null, string messageId = null, bool transliterate = false, DateTime? deliveryTime = null, TimeSpan? validalityPeriod = null, Dictionary<string, string> customParameters = null)
        {
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
                return SenderService.SendSms(session.UserId, session.SenderName, addresses, message, clientId: clientId, distributionId: distibutionId, smsId: messageId, transliterate: transliterate, deliveryTime: deliveryTime, validalityPeriod: validalityPeriod, customParameters: customParameters);
            else
                throw new Exception(Resources.Error_SessionIncorrect);
        }

        public List<SMSCheckItem> CheckSMSStatuses(string sessionKey, string clientId = null, string distributionId = null, DateTime? dateStart = null, DateTime? dateEnd = null, long? rowsPerPage = null, long? pageNumber = null, Dictionary<string, string> customLimits = null)
        {
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
                return SenderService.CheckSMSStatus(session.UserId, clientId: clientId, distibutionId: distributionId, rowsPerPage: rowsPerPage, pageNumber: pageNumber, customLimits: customLimits);
            else
                throw new Exception(Resources.Error_SessionIncorrect); 
        }

        public List<SMSCheckItem> CheckSMSByMessageId(string sessionKey, string messageId, long? rowsPerPage = null, long? pageNumber = null, string clientId = null)
        {
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
                return SenderService.CheckSMSStatus(session.UserId, extSmsId: messageId, rowsPerPage: rowsPerPage, pageNumber: pageNumber, clientId: clientId);
            else
                throw new Exception(Resources.Error_SessionIncorrect); 
        }

        public SMSCheckItem CheckSMSById(string sessionKey, Guid Id, string clientId = null)
        {
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
                return SenderService.CheckSMSStatus(session.UserId, id: Id, clientId: clientId).FirstOrDefault();
            else
                throw new Exception(Resources.Error_SessionIncorrect); 
        }

        public SMSCheckItem CheckSMS(string login, string password, Guid id, string clientId = null)
        {
            SMSCheckItem result = null;
            Guid? user = SenderService.Login(login, password);
            if (user != null)
                result = SenderService.CheckSMSStatus(user.Value, id: id, clientId: clientId).FirstOrDefault();
            else
                throw new Exception(Resources.Error_LoginFailed);
            return result;
        }

        public void LogOff(string sessionKey)
        {
            _sessionManager.KillSession(sessionKey);
        }

        public List<SMSCounter> GetSMSCounters(string sessionKey, SMSStatus? status = null, string clientId = null, string distibutionId = null, DateTime? dateStart = null, DateTime? dateEnd = null, Dictionary<string, string> customLimits = null)
        {
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
                return SenderService.GetSmsCounter(session.UserId, status: status, clientId: clientId, distibutionId: distibutionId, dateStart: dateStart, dateEnd:dateEnd, customLimits: customLimits);
            else
                throw new Exception(Resources.Error_SessionIncorrect);
        }

        public List<SMSDetail> GetSMSDetalization(string sessionKey, string clientId = null, string distibutionId = null, string extSmsId = null, Guid? id = null, DateTime? dateStart = null, DateTime? dateEnd = null, long? rowsPerPage = null, long? pageNumber = null, Dictionary<string, string> customLimits = null)
        {
            ServiceSession session = _sessionManager.GetSession(sessionKey);
            if (session != null)
                return SenderService.GetSmsDetalization(session.UserId, clientId: clientId, distibutionId: distibutionId, extSmsId: extSmsId, dateStart: dateStart, dateEnd: dateEnd, rowsPerPage: rowsPerPage, pageNumber: pageNumber, customLimits: customLimits);
            else
                throw new Exception(Resources.Error_SessionIncorrect);
        }

        #endregion
    }
}
