using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Csharper.OliverTwist.Repo;
using System.Web.Security;
using System.Web.SessionState;
using LinqKit;

namespace Csharper.OliverTwist.Model
{
    public class SessionAcessor
    {
        private HttpSessionState _session = null;

        private IClientRepo _repo = null;
        private ClientModel _realClient = null;
        private Guid _loginedUserId = Guid.Empty;
        private string _loginedUserName = string.Empty;
        private IMembershipService _membership = null;
        private decimal? _currentBallance;


        /// <summary>
        /// Текущее значение баланса клиента
        /// </summary>
        public decimal CurrentBallance
        {
            get
            {
                InitFields();
                return _currentBallance??0;
            }
        }

        /// <summary>
        /// Имя текущего залогиненого ПОЛЬЗОВАТЕЛЯ
        /// </summary>
        public string LoginedUserName
        {
            get
            {
                InitFields();
                return _loginedUserName;
            }
        }
        
        /// <summary>
        /// Id текщего залогиненого ПОЛЬЗОВАТЕЛЯ
        /// </summary>
        public Guid LoginedUserId
        {
            get
            {
                InitFields();
                return _loginedUserId;
            }
        }
        
        /// <summary>
        /// Текущий клиент от имени которого осуществляются действия
        /// </summary>
        public ClientModel OperationalClient
        {
             get
             {
                InitFields();
                return GetOperationalClient();
             }
            set
            {
                if (value != null && value.Id.HasValue)
                {
                    SetOperationalClient(value.Id.Value);
                }
            }
        }

        public void SetOperationalClient(long clientId)
        {
            _session["op_user_id"] = clientId;
        }

        private long? GetOperationalClientId()
        {
            long? result = null;
            if (_session != null && _session["op_user_id"] != null && _session["op_user_id"] is long)
            {
                result = (long)_session["op_user_id"];
            }
            if (result == null)
                result = _realClient.Id;
            return result;
        }

        private ClientModel GetOperationalClient()
        {
            ClientModel result = null;
            if (_session != null && _session["op_user_id"] != null && _session["op_user_id"] is long)
            {
                result = _repo.GetClientProjected((long)_session["op_user_id"]);
            }
            if (result == null)
                result = _realClient;
            return result;
        }

        /// <summary>
        /// Признак того, что производится работа под чужим клиентом
        /// </summary>
        public bool IsClientSwitched
        {
            get
            {
                InitFields();
                return RealClient == null ?
                    false : !(RealClient.Id == OperationalClient.Id);
            }
        }
        
        /// <summary>
        /// Клиент текущего залогиненого пользователя
        /// </summary>
        public ClientModel RealClient
        {
            get
            {
                InitFields();
                return _realClient;
            }
        }

        private SessionAcessor(HttpSessionState session, IMembershipService membership)
        {
            _session = session;
            _membership = membership;
        }

        public static SessionAcessor GetAcessor(IMembershipService membership)
        {
            return new SessionAcessor(HttpContext.Current.Session, membership);
        }

        public static SessionAcessor GetAcessor()
        {
            return new SessionAcessor(HttpContext.Current.Session, new AccountMembershipService());
        }

        private void InitFields()
        {
             MembershipUser user = _membership.GetUser();
             if (user != null && _loginedUserId.ToString() != user.ProviderUserKey.ToString())
             {
                 _loginedUserName = user.UserName;
                 _loginedUserId = new Guid(user.ProviderUserKey.ToString());
                 _realClient = ClientRepo.GetClientProjected(_loginedUserId);
                 if (_realClient != null)
                 {
                     _repo = RepoGetter<ClientRepo>.Get(_loginedUserName, _realClient.Id, GetOperationalClientId().Value);
                     _currentBallance = _realClient.Ballance;
                 }
             }
        }
    }
}
