using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Repo;
using Csharper.OliverTwist.Model.Extensions;
using System.Data;
using Csharper.OliverTwist.Model;
using System.Linq.Expressions;

namespace Csharper.OliverTwist.Repo
{
    public class ClientAccountRepo : RepoBase, IClientAccountRepo
    {
        protected ClientAccountRepo(string loginedUserName, long loginedClientId, long operationalClientId)
            : base(loginedUserName, loginedClientId, operationalClientId)
        {
        }

        #region IClientAccountRepo Members

        public ChangeClientAccountModel GetClientAccountChange(long clientId)
        {
            ChangeClientAccountModel result;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                result = (from client in DataContext.Clients
                          join account in DataContext.Accounts
                          on client.AccountId.GetValueOrDefault() equals account.Id
                          where client.Id == clientId && client.CreatedByClientId == OperationalClientId
                          select new ChangeClientAccountModel()
                            {
                                Id = account.Id,
                                CostRanges = account.CostRanges.Select(cr=>cr.ToModel()).ToList(),
                                Account = account.Amount
                            }
                            
                ).FirstOrDefault();
                tran.Commit();
            }
            return result;
        }

        public long? GetClientIdByAccountId(long accountId)
        {
            long? result;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                result = (from client in DataContext.Clients
                          join account in DataContext.Accounts
                          on client.AccountId.GetValueOrDefault() equals account.Id
                          where account.Id == accountId && (
                            client.CreatedByClientId == OperationalClientId ||
                            client.Id == OperationalClientId
                          )
                          select client.Id).FirstOrDefault();
                tran.Commit();
            }
            return result;
        }

        public ClientAccountModel GetClientAccountProjected(long accountId)
        {
            ClientAccountModel result;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                result = (from client in DataContext.Clients
                          join account in DataContext.Accounts
                          on client.AccountId.GetValueOrDefault() equals account.Id
                          where account.Id == accountId && (
                            client.CreatedByClientId == OperationalClientId ||
                            client.Id == OperationalClientId
                          )
                          select new ClientAccountModel()
                          {
                              Id = account.Id,
                              Account = account.Amount,
                              AccountType = account.AccountType,
                              DebtingType = account.DebtingType,
                              CostRanges = account.CostRanges.Select(cr => cr.ToModel()).ToList(),
                              Ballance = client.Account == null ? 0 :
                                (client.Account.AccountType == AccountType.PrePay ?
                                    DataContext.GetClearBallance(client.Id)
                                        : DataContext.GetClearBallance(client.Id) - DataContext.GetOverDraftBallance(client.Id))??0
                          }
                          ).FirstOrDefault();
                tran.Commit();
            }
            return result;
        }

        public bool ChangeAccount(ChangeClientAccountModel model)
        {
            return Convert.ToBoolean(DataContext.ChangeClientAccount(model.Id, model.AddingAmount, model.OneSMSCost, model.InputMoney, model.SelectedCostRangeId, model.Comment, (Guid)LoginedUser.ProviderUserKey, RealClientId, OperationalClientId));
        }

#warning Падает из за отсутствия синхронности
        public IQueryable<ClientAccountActionModel> GetClientAccountHistoryProjected(long clientId)
        {
            long accountId = GetAccountIdFromClientId(clientId);
            return DataContext.AccountHistories.Where(X => X.Id == accountId).Select(GetAccountActionModelExpression);
        }

        public Expression<Func<AccountHistory, ClientAccountActionModel>> GetAccountActionModelExpression
        {
            get
            {
                return accountAction => new ClientAccountActionModel()
                {
                    VersionDate = accountAction.VersionDate,
                    VersionId = accountAction.VersionId,
                    AccounId = accountAction.Id,
                    CostRangeId = accountAction.CostRangeId,
                    Amount = accountAction.Amount,
                    AmountDelta = accountAction.AmountDelta,
                    Comment = accountAction.Comment,
                    ManagerId = accountAction.ManagerId,
                    ManagerName = accountAction.ManagerId != null ? accountAction.ActionUser.UserName : string.Empty,
                    RealClientId = accountAction.RealClientId,
                    RealClientName = accountAction.RealClientId != null ? accountAction.ActionRealClient.OrganizationName : string.Empty,

                    OperationalClientId = accountAction.OperationalClientId,
                    OperationalClientName = accountAction.OperationalClientId != null ? accountAction.ActionOperationalClient.OrganizationName : string.Empty,

                    DistributionId = accountAction.DistributionId,

                    TargetAccountId = accountAction.TargetAccountId,
                    TargetAccountOrganizationName = accountAction.TargetAccountId != null?
                            DataContext.Clients.Where(X=>X.AccountId == accountAction.TargetAccountId).Select(X=>X.OrganizationName).FirstOrDefault():
                            string.Empty,

                    MoneyVolume = accountAction.MoneyVolume,
                    QuickCost = accountAction.QuickCost
                };
            }
        }

        private long GetAccountIdFromClientId(long clientId)
        {
            long accountId = DataContext.Clients.Where(X => X.Id == clientId).Select(X => X.AccountId).FirstOrDefault().Value;
            return accountId;
        }

        #endregion
    }
}


