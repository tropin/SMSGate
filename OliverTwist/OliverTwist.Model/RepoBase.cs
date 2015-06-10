using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model;
using System.Web.Security;

namespace Csharper.OliverTwist.Repo
{
    public class RepoBase
    {
        private long? _operationalClientId = null;
        private long? _loginedClientId = null;
        private string _loginedUserName = string.Empty;
        private static DBDataContext _dataContext = null;
        private IMembershipService _membershipServie = null;

        public IMembershipService MembershipService
        {
            get
            {
                if (_membershipServie == null)
                    _membershipServie = new AccountMembershipService();
                return _membershipServie;
            }
        }

        /// <summary>
        /// Идентификатор клиента от имени которого осуществляются действия
        /// </summary>
        protected long? OperationalClientId
        {
            get { return _operationalClientId; }
        }

        /// <summary>
        /// Контекст работы с базой данных
        /// </summary>
        protected static DBDataContext DataContext 
        {
            get
            {
                if (_dataContext == null)
                    _dataContext = new DBDataContext();
                if (_dataContext.Connection.State == System.Data.ConnectionState.Closed)
                    _dataContext.Connection.Open();
                return _dataContext;
            }
        }

        /// <summary>
        /// Идентификатор клиента который оригинально осущетсвляет изменения
        /// </summary>
        protected long? RealClientId
        {
            get { return _loginedClientId; }
        }

        /// <summary>
        /// Клиент от имени которого совершаются операции
        /// </summary>
        protected Client OperationalClient
        {
            get
            {
                Client result = null;
                if (_operationalClientId.HasValue)
                {
                    result = GetCleintById(_operationalClientId.Value);
                }
                return result;
            }
        }

        /// <summary>
        /// Клиент от которого оригинально совершаются действия
        /// </summary>
        protected Client RealClient
        {
            get
            {
                Client result = null;
                if (_loginedClientId.HasValue)
                {
                    result = GetCleintById(_loginedClientId.Value);
                }
                return result;
            }
        }

        /// <summary>
        /// Имя залогиненого пользователя
        /// </summary>
        protected string LoginedUserName
        {
            get
            {
                return _loginedUserName;
            }
        }

        /// <summary>
        /// Залогиненый пользователь
        /// </summary>
        protected MembershipUser LoginedUser
        {
            get
            {
                return string.IsNullOrEmpty(_loginedUserName)?
                    null : MembershipService.GetUser(_loginedUserName);
            }
        }



        /// <summary>
        ///  Конструктор репозитария
        /// </summary>
        /// <param name="operationalClientId">Id клиента от имени которого будут совершаться операции</param>
        /// <param name="realClientId">Id оригинально вошедшего клиента</param>
        /// <param name="LoginedName">Имя залогиненого пользователя</param>
        protected RepoBase(string loginedUserName, long? realClientId, long? operationalClientId)
        {
            _loginedUserName = loginedUserName;
            ReinitInstance(realClientId, operationalClientId);
        }

        private void ReinitInstance(long? realClientId, long? operationalClientId)
        {
            _loginedClientId = realClientId;
            _operationalClientId = operationalClientId;
        }

        protected Client GetCleintById(long id)
        {
            using (DBDataContext cxt = new DBDataContext())
            {
                if (cxt.Connection.State == System.Data.ConnectionState.Closed)
                    cxt.Connection.Open();
                using (var tran = cxt.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                    cxt.Transaction = tran;
                    var op_user = cxt.Clients.SingleOrDefault(user => user.Id == id);
                    return op_user;
                }
            }
        }
    }
}
