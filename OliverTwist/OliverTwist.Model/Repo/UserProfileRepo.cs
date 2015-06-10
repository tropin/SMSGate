using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model;
using System.Linq.Expressions;
using System.Data;

namespace Csharper.OliverTwist.Repo
{
    public class UserProfileRepo: RepoBase, IUserProfileRepo
    {
        #region IUserProfileRepo Members
        
        
        public UserProfileModel GetUserProfileProjected(Guid userId)
        {
            return GetAvailableProfiles().FirstOrDefault(profile => profile.UserId == userId);
        }

        public static List<string> GetAvailableRoles()
        {
            List<string> roles = null;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                DataContext.Transaction = tran;
                roles = DataContext.aspnet_Roles.Where(role => role.RoleName != RoleNames.ROOT_ADMIN)
                    .Select(role => role.RoleName).ToList();
                tran.Commit();
            }
            return roles;
        }

        protected UserProfileRepo(string loginedUserName, long loginedClientId, long operationalClientId)
            : base(loginedUserName, loginedClientId, operationalClientId)
        {
        }



        public IQueryable<UserProfileModel> GetAvailableProfiles(string roleFilter = "", int? childLevel = null, long? clientId = null)
        {   
            return from user in DataContext.aspnet_Users
                   join userDetails in DataContext.aspnet_Memberships
                   on user.UserId equals userDetails.UserId
                   join profile in DataContext.UserProfiles
                   on user.UserId equals profile.UserId
                   join clientAssigment in DataContext.Users2Clients
                   on user.UserId equals clientAssigment.UserId
                   join client in DataContext.Clients
                   on clientAssigment.ClientId equals client.Id
                   join underClient in DataContext.GetUnderControlClients(clientId??OperationalClientId)
                   on client.Id equals underClient.id
                   where !clientAssigment.IsDeleted &&
                         !client.IsDeleted &&
                         !profile.IsDeleted &&
                         (!childLevel.HasValue ||
                            (childLevel < 0 ? 
                                underClient.ChildLevel <= -childLevel:
                                underClient.ChildLevel == childLevel)
                          )  
                          &&
                         (roleFilter == string.Empty || 
                           user.RoleAssigments.Any(
                            assigment=>assigment.Role!=null && 
                            assigment.Role.RoleName.Contains(roleFilter)
                            )
                          )
                   select new UserProfileModel()
                   {
                       City = profile.City,
                       CreateDate = userDetails.CreateDate,
                       Email = userDetails.Email,
                       FirstName = profile.FirstName,
                       Id = profile.Id,
                       IsApproved = userDetails.IsApproved,
                       IsLockedOut = userDetails.IsLockedOut,
                       LastName = profile.LastName,
                       Login = user.UserName,
                       MiddleName = profile.MiddleName,
                       MobilePhone = profile.MobilePhone,
                       Roles = user.RoleAssigments.Select(ass => ass.Role.RoleName).ToList(),
                       Sex = profile.Sex,
                       TimeZone = profile.TimeZone,
                       UserId = user.UserId,
                       ClientId = client.Id,
                       ClientName = client.OrganizationName
                   };
        }

        public bool SaveUserProfile(UserProfileModel userProfile)
        {
            bool result = false;
            if (userProfile.Id.HasValue)
            {
                using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    DataContext.Transaction = tran;
                    var targetUser = DataContext.UserProfiles.FirstOrDefault(profile => profile.Id == userProfile.Id && !profile.IsDeleted &&
                            profile.User == null ?
                                false :
                                 profile.User.ClientAssigments == null ?
                                    false :
                                    profile.User.ClientAssigments.Client == null ?
                                        false :
                                        profile.User.ClientAssigments.Client.CreatedByClientId == OperationalClientId
                        );
                    if (targetUser != null)
                    {
                        targetUser.City = userProfile.City;
                        targetUser.FirstName = userProfile.FirstName;
                        targetUser.LastName = userProfile.LastName;
                        targetUser.MiddleName = userProfile.MiddleName;
                        targetUser.MobilePhone = userProfile.MobilePhone;
                        targetUser.Sex = userProfile.Sex;
                        targetUser.TimeZone = userProfile.TimeZone;
                        if (targetUser.User != null && targetUser.User.Membership != null)
                            targetUser.User.Membership.Email = userProfile.Email;
                        IEnumerable<string> currentRoles = targetUser.User.RoleAssigments.Where(role => role.Role != null)
                                                      .Select(role => role.Role.RoleName);
                        var deleting = currentRoles.Except(userProfile.Roles);
                        var adding = userProfile.Roles.Except(currentRoles);
                        if (deleting.Count() > 0)
                        {
                            DataContext.aspnet_UsersInRoles.DeleteAllOnSubmit(
                                DataContext.aspnet_UsersInRoles.Where(assigment => assigment.Role != null && deleting.Contains(assigment.Role.RoleName))
                                );
                        }
                        if (adding.Count() > 0)
                        {
                            Func<string, Guid, aspnet_UsersInRole> addingAction = delegate(string roleName, Guid userId)
                            {
                                aspnet_UsersInRole addresult = null;
                                var role = DataContext.aspnet_Roles.FirstOrDefault(rl => rl.RoleName == roleName);
                                if (role != null)
                                {
                                    addresult = new aspnet_UsersInRole()
                                    {
                                        RoleId = role.RoleId,
                                        UserId = userId
                                    };
                                }
                                return addresult;
                            };
                            DataContext.aspnet_UsersInRoles.InsertAllOnSubmit(
                                    adding.Select
                                    (
                                        addRole => addingAction(addRole, targetUser.UserId)
                                    ).Where(el => el != null)
                                );
                        }
                        DataContext.SubmitChanges();
                    }
                    tran.Commit();
                }
            }
            return result;
        }

        public bool DeleteUser(Guid userId)
        {
            bool result = false;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                DataContext.Transaction = tran;
                var deletingUser = DataContext.aspnet_Users.FirstOrDefault(user => user.UserId == userId);
                if (deletingUser != null)
                {
                    try
                    {
                        if (deletingUser.Profile!=null)
                            deletingUser.Profile.IsDeleted = true;
                        deletingUser.Membership.IsLockedOut = true;
                        if (deletingUser.ClientAssigments != null)
                            deletingUser.ClientAssigments.IsDeleted = true;
                        DataContext.SubmitChanges();
                        result = true;
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
                tran.Commit();
            }
            return result;
        }

        public IQueryable<UserProfileModel> GetUsersProjected(long? clientId, string roleFilter, int? childLevel)
        {
            IQueryable<UserProfileModel> result = null;
            using (var tran = DataContext.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                result = GetAvailableProfiles(roleFilter, childLevel, clientId);
                tran.Commit();
            }
            return result;
        }

        #endregion
    }
}
