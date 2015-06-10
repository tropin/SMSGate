using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model;
using OliverTwist.Converters;
using Csharper.OliverTwist.Model.Extensions;
using System.Linq.Expressions;
using LinqKit;
using System.Web.Security;
using System.Data;
using System.Data.Linq;
using System.Diagnostics;

namespace Csharper.OliverTwist.Repo
{
    public class ClientRepo: RepoBase, IClientRepo
    {
        public static Expression<Func<Client, ClientModel>> ClientModelSelectExpression
        {
            get
            {
                return client => new ClientModel()
                {
                    Id = client.Id == 0 ? (long?)null : client.Id,
                    Login = client.UserAssigments.Where(assigment => !assigment.IsDeleted).Select(assigment => assigment.User.UserName).ToList(),
                    OrganizationName = client.OrganizationName,
                    Status = client.Status,
                    IsDealler = client.DeallerOfClientId != null,
                    Account = client.Account == null ? 0 : client.Account.Amount,
                    Ballance = client.Account == null ? 0 :
                        client.Account.AccountType == AccountType.PrePay?
                           DataContext.GetClearBallance(client.Id) :
                           DataContext.GetClearBallance(client.Id) - DataContext.GetOverDraftBallance(client.Id),
                    CostRanges = client.Account.CostRanges.Select(cr => cr.ToModel()).ToList(),
                    DebtingType = client.Account.DebtingType
                };
            }
        }
        
        #region IClientRepo Members

        protected ClientRepo(string loginedUserName, long? loginedClientId, long? operationalClientId)
            : base(loginedUserName, loginedClientId, operationalClientId)
        {
        }

        public ClientModel GetClientProjected(long clientId)
        {
            ClientModel result = null;
            Client fetched = GetClient(clientId);
            if (fetched != null)
                result = ClientModelSelectExpression.Invoke(fetched);
            return result;
        }

        public static ClientModel GetClientProjected(Guid userId)
        {
            ClientModel result = null;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                result = (from client in DataContext.Clients
                         join assigment in DataContext.Users2Clients
                         on client.Id equals assigment.ClientId
                         join user in DataContext.aspnet_Users
                         on assigment.UserId equals user.UserId
                         where user.UserId == userId select client
                         ).Select(ClientModelSelectExpression).FirstOrDefault();
                tran.Commit();
            }
            return result;
        }

        public static ClientModel GetClientConcrete(long clientId)
        {
            ClientModel result = null;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                result = (from client in DataContext.Clients
                          join assigment in DataContext.Users2Clients
                          on client.Id equals assigment.ClientId
                          where client.Id == clientId
                          select client
                         ).Select(ClientModelSelectExpression).FirstOrDefault();
                tran.Commit();
            }
            return result;
        }

        
        public Client GetClient(long clientId)
        {
            Client result = null;                
                using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    DataContext.Transaction = tran;
                    result = DataContext.Clients.SingleOrDefault(client => !client.IsDeleted && client.Id == clientId && 
                        ((client.CreatedByClientId == OperationalClientId) ||
                          clientId == OperationalClientId)
                        );
                    tran.Commit();
                }
            return result;
        }

        public IQueryable<Client> GetClients()
        {
            IQueryable<Client> result = new List<Client>().AsQueryable();
            if (LoginedUser != null)
            {
                using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    DataContext.Transaction = tran;
                    result = from client in DataContext.Clients
                             where !client.IsDeleted && client.CreatedByClientId == OperationalClientId ||
                                (Roles.IsUserInRole(LoginedUserName, RoleNames.ROOT_ADMIN) && client.CreatedByClientId == null)
                             select client;
                    tran.Commit();
                }
            }
            return result;
        }

        public IQueryable<ClientModel> GetClientsProjected(string loginToSearch)
        {
            return (from client in GetClients()
                   join assigment in DataContext.Users2Clients
                   on client.Id equals assigment.ClientId
                   join user in DataContext.aspnet_Users
                   on assigment.UserId equals user.UserId
                   where string.IsNullOrEmpty(loginToSearch) || user.LoweredUserName.Contains(loginToSearch)
                   select client).Select(ClientModelSelectExpression);
        }

        public ClientModel CreateClient(UserProfileModel regInfo, string organiztionName, MembershipUser newUser, long? templateClientAccountId = null, ClientStatus clientStatus = ClientStatus.NotActive, bool isDealler = false)
        {
            ClientModel model = null;
             if (newUser!=null && regInfo!=null)
             {
                 Guid userId = (Guid)newUser.ProviderUserKey;
                 try
                 {
                     List<Client> client = new List<Client>(DataContext.CreateClient(userId,
                         organiztionName,
                         isDealler,
                         this.OperationalClientId,
                         templateClientAccountId == null ? BallancePolicy.StartupBallance : (decimal?)null,
                         templateClientAccountId == null ? BallancePolicy.StartupMessageCost : (decimal?)null,
                         templateClientAccountId == null ? (int)BallancePolicy.DefaultAccountType : (int?)null,
                         templateClientAccountId == null ? (int)BallancePolicy.DefaultDebtingType : (int?)null,
                         templateClientAccountId,
                         (int)clientStatus,
                         regInfo.FirstName,
                         regInfo.MiddleName,
                         regInfo.LastName,
                         regInfo.City,
                         regInfo.MobilePhone,
                         regInfo.TimeZone,
                         (int)(regInfo.Sex??Sex.Unknown)));
                     if (client.Count> 0)
                         model = ClientModelSelectExpression.Invoke(client.FirstOrDefault());
                 }
                 catch(Exception ex)
                 {
                     Trace.TraceError(ex.ToString());
                     DataContext.DeleteUserInternal(userId);
                     throw;
                 }
             }
             return model;
        }

        public void UpdateClient(ClientModel client)
        {
            using (var tran = DataContext.Connection.BeginTransaction())
            {
                DataContext.Transaction = tran;
                Client clientToModify = null;
                if (client != null && client.Id.HasValue)
                {
                    Client fetched = DataContext.Clients.FirstOrDefault(row => !row.IsDeleted && row.Id == client.Id.Value);
                    if (fetched != null)
                        clientToModify = fetched;
                    else
                        return;
                }
                if (clientToModify == null || client == null)
                    return;
                client.ToDSO(clientToModify, OperationalClient);
                tran.Commit();
                UpdateClient(clientToModify, HistoryAction.Change);
            }
        }

        private void UpdateClient(Client client, HistoryAction action)
        {
            using (var tran = DataContext.Connection.BeginTransaction())
            {
                DataContext.Transaction = tran;
                ClientsHistory hst = new ClientsHistory()
                                {
                                    AccountId = client.AccountId,
                                    Action = action,
                                    CreatedByClientId = client.CreatedByClientId,
                                    DateCreated = client.DateCreated,
                                    DeallerOfClientId = client.DeallerOfClientId,
                                    Id = client.Id,
                                    IsDeleted = client.IsDeleted,
                                    OrganizationName = client.OrganizationName,
                                    Status = client.Status,
                                    ManagerId = LoginedUser == null ? (Guid?)null : (Guid)LoginedUser.ProviderUserKey,
                                    OperationalClientId = OperationalClientId,
                                    RealClientId = RealClientId,
                                    VersionDate = DateTime.Now
                                };
                DataContext.ClientsHistories.InsertOnSubmit(hst);
                DataContext.SubmitChanges();
                tran.Commit();
            }
        }

        public void DeleteClient(long id)
        {
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                Client fetched = DataContext.Clients.FirstOrDefault(row => row.Id == id && row.CreatedByClientId == OperationalClientId && !row.IsDeleted);
                tran.Commit();
                if (fetched != null)
                {
                    fetched.IsDeleted = true;
                    UpdateClient(fetched, HistoryAction.Delete);
                }
            }
        }

        public void ActivateClientForUser(Guid userId)
        {
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                aspnet_User aspUser = DataContext.aspnet_Users.FirstOrDefault(user => user.UserId == userId);
                tran.Commit();
                if (aspUser != null)
                {
                    if (aspUser.ClientAssigments != null && aspUser.ClientAssigments.ClientId.HasValue && aspUser.ClientAssigments.Client.UserAssigments.Count == 1)
                    {
                        ActivateClient(aspUser.ClientAssigments.ClientId.Value);
                    }
                }
            }
        }

        public ClientModel GetClientForUser(Guid userId)
        {
            ClientModel model = null;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                aspnet_User aspUser = DataContext.aspnet_Users.FirstOrDefault(user => user.UserId == userId);
                tran.Commit();
                if (aspUser != null && aspUser.ClientAssigments!=null && aspUser.ClientAssigments.ClientId!=null)
                {
                    model = ClientModelSelectExpression.Invoke(aspUser.ClientAssigments.Client);
                }
            }
            return model;
        }

        public void ActivateClient(long id)
        {
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                Client fetched = DataContext.Clients.FirstOrDefault(row => row.Id == id && !row.IsDeleted);
                tran.Commit();
                if (fetched != null)
                {
                    fetched.Status = ClientStatus.Active;
                    UpdateClient(fetched, HistoryAction.Change);
                }
            }
        }

        public bool IsCanSetClientAsCurrent(long clientId)
        {
            bool result = false;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                
                DataContext.Transaction = tran;
                var client = (from currClient in DataContext.GetUnderControlClients(OperationalClientId)
                              join cl in DataContext.Clients
                              on currClient.id equals cl.Id
                              where currClient.id == clientId && !cl.IsDeleted
                              select cl).FirstOrDefault();

                if (client != null)
                    result = true;
                tran.Commit();
            }
            return result;
        }

        #endregion
    }
}
