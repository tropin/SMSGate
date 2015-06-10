using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model;

namespace Csharper.OliverTwist.Repo
{
    public interface IUserProfileRepo
    {
        UserProfileModel GetUserProfileProjected(Guid userId);
        bool SaveUserProfile(UserProfileModel userProfile);
        bool DeleteUser(Guid userId);
        IQueryable<UserProfileModel> GetUsersProjected(long? clientId, string roleFilter,int? childLevel);
    }
}
