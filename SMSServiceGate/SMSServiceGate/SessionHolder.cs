using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using Csharper.SMSServiceGate.Properties;
using SmsService = Csharper.SMSServiceGate.ADEService.ADEService;

namespace Csharper.SMSServiceGate
{
    public class SessionHolder
    {
        private SmsService service = new SmsService();
        
        private List<SessinInfo> _activeSessions = new List<SessinInfo>();

        public string GetSessionKey(string login, string password, string senderName)
        {
            string result = string.Empty;
            MD5 md = MD5.Create();
            string uqHash = Encoding.UTF8.GetString(md.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(login,password))));  
            var sessions = _activeSessions.Where(sess => sess.UniquHash == uqHash);
            if (sessions.Count() > 0)
            {
                SessinInfo session = sessions.First();
                if (session.TimeToKill < DateTime.Now)
                    result = session.SessionKey;
                else
                {
                    try
                    {
                        service.LogOff(session.SessionKey);
                    }
                    catch {/*Если сервис не съест, нам пофиг */ }
                    _activeSessions.Remove(session);
                    result = service.Login(login, password, senderName);
                    if (!string.IsNullOrEmpty(result))
                    {
                        _activeSessions.Add(new SessinInfo()
                        {
                            SessionKey = result,
                            TimeToKill = DateTime.Now.AddMinutes(Settings.Default.SessionTTLMin),
                            UniquHash = uqHash
                        });
                    }
                }
            }
            else
            {
                result = service.Login(login, password, senderName);
                if (!string.IsNullOrEmpty(result))
                {
                    _activeSessions.Add(new SessinInfo()
                    {
                        SessionKey = result,
                        TimeToKill = DateTime.Now.AddMinutes(Settings.Default.SessionTTLMin),
                        UniquHash = uqHash
                    }
                    );
                }
            }
            return result;
        }
    }
}