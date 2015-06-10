using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using System.Threading;

namespace Csharper.Common
{
    public class SessionManager<T>
        where T: class, ISession, new()
    {
        private const int SESSION_KEY_LENGTH = 32;

        private Dictionary<string, T> _sessions
            = new Dictionary<string, T>();

        private Dictionary<string, string> _sessionKeys
            = new Dictionary<string, string>();

        private Timer _sessionTimer;

        private object _sessionLock = new object();

        private TimeSpan _sessionTTL;

        public SessionManager(TimeSpan sessionTTL)
        {
            _sessionTTL = sessionTTL;
            _sessionTimer = new Timer(SessionsTTLCheck, null, TimeSpan.Zero, sessionTTL);
        }

        private void SessionsTTLCheck(object state)
        {
            lock (_sessionLock)
            {
                IEnumerable<string> sKeys = _sessions.Where(session => DateTime.Now - session.Value.LastAccess > _sessionTTL)
                                                     .Select(session => session.Value.SessionKey);
                foreach (string sKey in sKeys)
                {
                    KillSessionInternal(sKey);
                }
            }
        }

        public T GetSession(string sessionKey)
        {
            T session = null;
            lock (_sessionLock)
            {
                if (_sessions.ContainsKey(sessionKey))
                {
                    session = _sessions[sessionKey];
                    session.LastAccess = DateTime.Now;
                }
            }
            return session;
        }

        public bool KillSession(string sessionKey)
        {
            lock (_sessionLock)
            {
                return KillSessionInternal(sessionKey);
            }
        }

        private bool KillSessionInternal(string sessionKey)
        {
            bool result = false;
            T session = null;
            if (_sessions.ContainsKey(sessionKey))
                session = _sessions[sessionKey];
            if (session != null)
            {
                var sessionUniqueKey = _sessionKeys.Where(key => key.Value == sessionKey).FirstOrDefault();
                if (sessionKey != null)
                {
                    _sessionKeys.Remove(sessionUniqueKey.Key);
                }
                result = _sessions.Remove(sessionKey);
            }
            return result;
        }

        private static List<PropertyInfo> _uniqueProperties;
        
        private List<PropertyInfo> UniqueProperties
        {
            get
            {
                lock(_sessionLock)
                {
                    if (_uniqueProperties == null)
                    {
                        _uniqueProperties = new List<PropertyInfo>();
                        PropertyInfo[] infos = typeof(T).GetProperties();
                        foreach (PropertyInfo pi in infos)
                        {
                            if (pi.GetCustomAttributes(typeof(SessionUniqueAttribute), true).Count()>0)
                            {
                                _uniqueProperties.Add(pi);
                            }
                        }
                    }
                }
                return _uniqueProperties;
            }
        }

        public T GetSession(T draftSession)
        {
            T result = null;
            byte[] key = GetSessionUniqueKey(draftSession);
            lock (_sessionLock)
            {
                string strKey = key.Stringify();
                if (!_sessionKeys.ContainsKey(strKey))
                {
                    byte[] sessionKey = GetRandomKey(SESSION_KEY_LENGTH);
                    string strSessionKey = sessionKey.Stringify();
                    draftSession.SessionKey = strSessionKey;
                    draftSession.LastAccess = DateTime.Now;
                    _sessions.Add(strSessionKey, draftSession);
                    _sessionKeys.Add(strKey, strSessionKey);
                }
                result = _sessions[_sessionKeys[strKey]];
                result.LastAccess = DateTime.Now;
            }
            return result;
        }

        /// <summary>
        /// Получить уникальный ключ сессии
        /// </summary>
        /// <param name="draftSession">Черновая сессия</param>
        /// <returns></returns>
        public byte[] GetSessionUniqueKey(T draftSession)
        {
            string tempKey = string.Empty;
            foreach(PropertyInfo pi in UniqueProperties)
            {
                 tempKey = string.Format("{0}_{1}",tempKey, pi.GetValue(draftSession, null));
            }
            
            MD5 md = MD5.Create();
            return md.ComputeHash(
                Encoding.UTF8.GetBytes(tempKey)
                );
        }

        /// <summary>
        /// Получить произвольный ключ сессии
        /// </summary>
        /// <param name="bytelength">Количество бит к ключе</param>
        /// <returns></returns>
        public static byte[] GetRandomKey(int bytelength)
        {
            byte[] buff = new byte[bytelength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buff);
            return buff;
        }
    }
}
