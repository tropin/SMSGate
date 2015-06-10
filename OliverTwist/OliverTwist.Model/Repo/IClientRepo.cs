using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model;
using System.Web.Security;

namespace Csharper.OliverTwist.Repo
{
    public interface IClientRepo
    {
        ClientModel GetClientProjected(long clientId);
        IQueryable<ClientModel> GetClientsProjected(string loginFilter);
        void UpdateClient(ClientModel client);
        ClientModel CreateClient(UserProfileModel regInfo, string organiztionName, MembershipUser newUser, long? templateClientAccountId = null, ClientStatus clientStatus = ClientStatus.NotActive, bool isDealler = false);
        void DeleteClient(long clentId);
        void ActivateClient(long clientId);
        void ActivateClientForUser(Guid userId);
        ClientModel GetClientForUser(Guid userId);
        bool IsCanSetClientAsCurrent(long clientId);
    }
}
