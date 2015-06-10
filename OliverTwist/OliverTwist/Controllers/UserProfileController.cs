using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Csharper.OliverTwist.Repo;
using Csharper.OliverTwist.Model;
using System.Security.Cryptography;
using System.Net.Mail;
using MvcFlan;
using MvcFlan.PagingSorting;
using OliverTwist.FilterContainers;
using MvcContrib.UI.Grid;

namespace OliverTwist.Controllers
{

    [HandleError]
    public class UserProfileController : OTController
    {
        private IUserProfileRepo _profileRepo;

        private IUserProfileRepo ProfileRepo
        {
            get
            {
                return _profileRepo;
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (OTSession.OperationalClient != null)
            {
                _profileRepo = RepoGetter<UserProfileRepo>.Get(OTSession.LoginedUserName, OTSession.RealClient.Id, OTSession.OperationalClient.Id);
            }
        }

        [NonAction]
        private ListContainerModel<UserProfileModel, UserProfilesFilterContainer> GetUsersList(UserProfileModel profileFilters, PageSortOptions pageSortOptions, long? clientId = null, int? childLevel = null)
        {
            string roleToSearch = (profileFilters.Roles ?? new List<string>()).FirstOrDefault()??string.Empty;
            var clientPagedList = ProfileRepo.GetUsersProjected(clientId, roleToSearch, childLevel).AsFiltered(profileFilters)
                .AsPagination(pageSortOptions);
            var userProfileFilterContainer = new UserProfilesFilterContainer()
            {
                Login = profileFilters.Login,
                City = profileFilters.City,
                ClientName = profileFilters.ClientName,
                CreateDate = profileFilters.CreateDate,
                Email = profileFilters.Email,
                IsApproved = profileFilters.IsApproved,
                IsLockedOut = profileFilters.IsLockedOut,
                LastName = profileFilters.LastName,
                MobilePhone = profileFilters.MobilePhone,
                Roles = roleToSearch,
                Sex = profileFilters.Sex,
                TimeZone = profileFilters.TimeZone
            };

            var gridSortOptions = new GridSortOptions
            {
                Column = pageSortOptions.Column,
                Direction = pageSortOptions.Direction
            };

            var clientListContainer = new ListContainerModel<UserProfileModel, UserProfilesFilterContainer>()
            {
                PagedList = clientPagedList,
                FilterContainer = userProfileFilterContainer,
                GridSortOptions = gridSortOptions
            };
            return clientListContainer;
        }

        /// <summary>
        /// Вывод списка клиентов с фильтрацией и сортировкой
        /// </summary>
        /// <param name="clientFilters">Настройки фильтров</param>
        /// <param name="pageSortOptions">Опции сортировки</param>
        /// <returns>Новый список с настройками</returns>
        [Authorize]
        public ActionResult Index(UserProfileModel clientFilters, PageSortOptions pageSortOptions, int? childLevel)
        {
            return View(GetUsersList(clientFilters, pageSortOptions, childLevel: childLevel));
        }

        [Authorize]
        public PartialViewResult GetUsersForClient(UserProfileModel clientFilters, PageSortOptions pageSortOptions, long clientId, int? childLevel)
        {
            return PartialView("SearchResultsWithPaging",GetUsersList(clientFilters, pageSortOptions, clientId));
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateNew()
        {
            UserProfileModel emptyModel = new UserProfileModel();
            return View("Details", emptyModel);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Details(Guid id)
        {
            UserProfileModel userModel = ProfileRepo.GetUserProfileProjected(id);
            return View("Details", userModel);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateNew(UserProfileModel newUserProfile)
        {
            return Details(newUserProfile);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Details(UserProfileModel userProfile)
        {
            if (ModelState.IsValid)
            {
                ProfileRepo.SaveUserProfile(userProfile);
                return RedirectToAction("Index");
            }
            // Если что-то не сошлось непоказываем форму
            return View("Details", userProfile);
        }


        [Authorize]
        public PartialViewResult DeleteUser(string options, int page, Guid id)
        {
            ProfileRepo.DeleteUser(id);
            OptionsHolder<UserProfileModel> holder = OptionsHolder<UserProfileModel>.GetHolder(options.Replace("\"\"", "null"));
            PageSortOptions pso = new PageSortOptions()
            {
                Column = holder.Sort.Column,
                Direction = holder.Sort.Direction,
                Page = page
            };
            return PartialView("SearchResultsWithPaging", GetUsersList(holder.Filter, pso));
        }

        
    }
}
