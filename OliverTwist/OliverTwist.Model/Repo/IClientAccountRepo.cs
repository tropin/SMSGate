using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model;

namespace Csharper.OliverTwist.Repo
{
    public interface IClientAccountRepo
    {
        ChangeClientAccountModel GetClientAccountChange(long clientId);
        long? GetClientIdByAccountId(long accountId);
        ClientAccountModel GetClientAccountProjected(long accountId);
        IQueryable<ClientAccountActionModel> GetClientAccountHistoryProjected(long clientId);
        bool ChangeAccount(ChangeClientAccountModel model);
    }
}
