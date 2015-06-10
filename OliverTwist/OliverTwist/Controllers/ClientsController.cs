using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Csharper.OliverTwist.Repo;
using System.Web.Security;
using Csharper.OliverTwist.Model;
using MvcFlan.PagingSorting;
using OliverTwist.FilterContainers;
using MvcContrib.UI.Grid;
using MvcFlan;
using Csharper.OliverTwist.Model.Extensions;
using System.Web.Script.Serialization;
using System.Data;
using System.Diagnostics;

namespace OliverTwist.Controllers
{
    [HandleError]
    public class ClientsController : OTController
    {
        private IClientRepo _clientRepo;

        private IClientRepo ClientRepo
        {
            get
            {
                return _clientRepo;
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (OTSession.OperationalClient != null)
            {
                _clientRepo = RepoGetter<ClientRepo>.Get(OTSession.LoginedUserName, OTSession.RealClient.Id, OTSession.OperationalClient.Id);
            }
        }
       
        [Authorize]
        public PartialViewResult DeleteClient(string options, int page, long id)
        {
            ClientRepo.DeleteClient(id);
            OptionsHolder<ClientModel> holder = OptionsHolder<ClientModel>.GetHolder(options); 
            PageSortOptions pso = new PageSortOptions()
            {
                Column = holder.Sort.Column,
                Direction = holder.Sort.Direction,
                Page = page
            };
            return PartialView("SearchResultsWithPaging", GetClientsList(holder.Filter, pso));
        }

        [NonAction]
        private ListContainerModel<ClientModel, ClientsFilterContainer> GetClientsList(ClientModel clientFilters, PageSortOptions pageSortOptions)
        {
            string loginToSearch = (clientFilters.Login ?? new List<string>()).FirstOrDefault();
            var clientPagedList = ClientRepo.GetClientsProjected(loginToSearch).AsFiltered(clientFilters)
                .AsPagination(pageSortOptions);
            var clientViewFilterContainer = new ClientsFilterContainer()
                {
                    Login = loginToSearch,
                    IsDealler = clientFilters.IsDealler,
                    OrganizationName = clientFilters.OrganizationName,
                    Status = clientFilters.Status,
                };
            
            var gridSortOptions = new GridSortOptions 
            { 
                Column = pageSortOptions.Column, 
                Direction = pageSortOptions.Direction 
            };

            var clientListContainer = new ListContainerModel<ClientModel,ClientsFilterContainer>() 
            {
                PagedList = clientPagedList,
                FilterContainer = clientViewFilterContainer,
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
        [HttpGet]
        public ActionResult Index(ClientModel clientFilters, PageSortOptions pageSortOptions)
        {
            return View(GetClientsList(clientFilters, pageSortOptions));
        }

        [HttpPost]
        public ActionResult Index(MembershipUser user, ClientModel client)
        {
            ViewData["InvalidSendUserName"] = user.UserName;
            ViewData["InvalidSendEmail"] = user.Email;
            ViewData["InvalidSendClientName"] = client.OrganizationName;
            ViewData["InvalidSendClientId"] = client.Id;
            return View(GetClientsList(new ClientModel(), new PageSortOptions()));
        }

        [Authorize]
        public ActionResult Details(long id)
        {
            return View(ClientRepo.GetClientProjected(id));
        }
        [Authorize]
        public ActionResult CreateNew()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult CreateNew(NewClientModel model)
        {
            MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, Membership.GeneratePassword(MembershipService.MinPasswordLength, MembershipService.MinRequiredNonAlphanumericCharacters), model.Email);
            if (createStatus == MembershipCreateStatus.Success)
            {
                //Если пользователь зарегистрировался, то для своего клиента он Админ
                Roles.AddUserToRole(model.UserName, RoleNames.ADMIN);
                //Создаем клиента
                MembershipUser user = Membership.GetUser(model.UserName);
                ClientModel client = null;
                try
                {
                    client = ClientRepo.CreateClient(model, model.OrganizationName, user, null, model.Status, model.IsDealler);
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);   
                }
                if (client != null)
                {
                    bool isMailError = false;
                    try
                    {
                        MailGenerator.Mailer.Send(MailGenerator.GetClientInviteMail(user, OTSession.OperationalClient.OrganizationName, Request.RequestContext));
                    }
                    catch (Exception ex)
                    {
                        isMailError = true;
                        Trace.TraceError("Ошибка отправки уведомления об отправке пользователь {0} email {1}, ошибка {2}", user.UserName, user.Email, ex);
                    }
                    return RedirectToAction("Index", isMailError?new {User = user, Client = client}: null);
                }
                else return View(model);
            }
            else
            {
                  ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
            }
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Details(ClientModel model)
        {
            if (ModelState.IsValid)
            {
                ClientRepo.UpdateClient(model);
                return RedirectToAction("Index");
            }
            // Если что-то не сошлось непоказываем форму
            return View("Details", model);
        }

        [Authorize]
        public ActionResult SetCurrent(long id)
        {
            if (ClientRepo.IsCanSetClientAsCurrent(id))
            {
                OTSession.SetOperationalClient(id);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult RevertCurrent()
        {
            OTSession.OperationalClient = OTSession.RealClient;
            return RedirectToAction("Index");
        }
    }
}
