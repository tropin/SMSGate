using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Csharper.OliverTwist.Model;
using Csharper.OliverTwist.Repo;
using Csharper.OliverTwist;
using MvcFlan;
using OliverTwist.FilterContainers;
using MvcContrib.UI.Grid;
using MvcContrib;
using MvcFlan.PagingSorting;
using System.Dynamic;

namespace OliverTwist.Controllers
{
    [HandleError]
    public class ClientAccountsController : OTController
    {
        private IClientAccountRepo _clientAccountRepo;

        private IClientAccountRepo ClientAccountRepo
        {
            get
            {
                return _clientAccountRepo;
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (OTSession.OperationalClient != null)
            {
                _clientAccountRepo = RepoGetter<ClientAccountRepo>.Get(OTSession.LoginedUserName, OTSession.RealClient.Id, OTSession.OperationalClient.Id);
            }
        }

        private bool ValidateAccountChangeAction(long clientId)
        {
            bool result = false;
            // если клиент в принципе дилер
            if (OTSession.OperationalClient.IsDealler.HasValue && OTSession.OperationalClient.IsDealler.Value)
            {
                // если клиент собирается менять не свой собственный счет
                if (clientId != OTSession.OperationalClient.Id)
                {
                    result = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Вы не можете изменять свой счет.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Вы не можете изменять счета клиентов, т.к. не являетесь дилером.");
            }
            return result;
        }
        
        private ChangeClientAccountModel GetClientAccountChangeModel(long clientId)
        {
            ChangeClientAccountModel model = null;
            if (ValidateAccountChangeAction(clientId))
            {
                //Возвращает null если записи не найдены
                 model = ClientAccountRepo.GetClientAccountChange(clientId);
                 if (model == null)
                 {
                      ModelState.AddModelError(string.Empty, "Вы не можете изменять счет чужого клиента.");
                 }
            }
          
            if (model == null)
            {
                model = new ChangeClientAccountModel();
            }
            return model;
        }

        /// <summary>
        /// Получение частичного View редактирования счета
        /// </summary>
        /// <param name="clientId">Id клиента, счет которого надо отредактировать</param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult ChangeClientAccount(long clientId)
        {
            ChangeClientAccountModel model = GetClientAccountChangeModel(clientId);
            return PartialView("ChangeClientAccount", model);
        }

        /// <summary>
        /// Зачисление на счет.
        /// </summary>
        /// <param name="model">Модель смены счета</param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult ChangeClientAccount(ChangeClientAccountModel model)
        {
            bool isOperationValid = true;
            if (model.Id.HasValue && ModelState.IsValid) //Счет должен быть известен, без вариантов 
            {
                // Получаем Id клиента по Id его счета
                long? clientId = ClientAccountRepo.GetClientIdByAccountId(model.Id.Value);

                // Еще раз делаем проверку - является ли клиент дилером и не изменяет свой счет.
                if (clientId.HasValue && ValidateAccountChangeAction(clientId.Value))
                {
                    ChangeClientAccountModel emptyModel = GetClientAccountChangeModel(clientId.Value);
                    if (emptyModel != null) //Только если мы точно уверены в Id счета и легитимности его изменения
                    {
                        CostCalculator calc = new CostCalculator(model.CalcMode);
                        calc.RecalcModel(model);
                        isOperationValid = ClientAccountRepo.ChangeAccount(model);
                        if (isOperationValid)
                        {
                            model = emptyModel;
                            ViewData.Add("AccountUpdated", true);
                        }
                    }
                }
            }
            return PartialView("ChangeClientAccount", model);
        }

        [NonAction]
        private ListContainerModel<ClientAccountActionModel, AccountHistoryFilterContainer> 
            GetHistory(long clientId, ClientAccountActionModel accountFilters, PageSortOptions pageSortOptions)
        {
            var accountHistoryList = ClientAccountRepo.GetClientAccountHistoryProjected(clientId)
                .AsFiltered(accountFilters)
                .AsPagination(pageSortOptions);

            var accountHistoryViewFilterContainer = new AccountHistoryFilterContainer()
            {
                VersionDate = accountFilters.VersionDate,
                ManagerName = accountFilters.ManagerName,
                RealClientName = accountFilters.RealClientName,
                OperationalClientName = accountFilters.OperationalClientName,
                MoneyVolume = accountFilters.MoneyVolume,
                QuickCost = accountFilters.QuickCost,
                TargetAccountOrganizationName = accountFilters.TargetAccountOrganizationName
            };

            var gridSortOptions = new GridSortOptions
            {
                Column = pageSortOptions.Column,
                Direction = pageSortOptions.Direction
            };

            var accountHistoryListContainer = new ListContainerModel<ClientAccountActionModel, AccountHistoryFilterContainer>()
            {
                PagedList = accountHistoryList,
                FilterContainer = accountHistoryViewFilterContainer,
                GridSortOptions = gridSortOptions
            };
            return accountHistoryListContainer;
        }

 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult AccountHistoryIndex(long clientId, ClientAccountActionModel accountFilters, PageSortOptions pageSortOptions)
        {
            var accountHistory = GetHistory(clientId, accountFilters, pageSortOptions);
            dynamic parameter = new ExpandoObject();
            parameter.AccountHistory = accountHistory;
            parameter.ClientId = clientId;
            //Все последующие запросы направляем сюда (блять строчка на 3 часа, ебнуцо)
            ControllerContext.RouteData.Values["action"] = "AccountHistorySearchIndex";
            return PartialView("AccountHistoryIndex", parameter); 
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult AccountHistorySearchIndex(long clientId, ClientAccountActionModel accountFilters, PageSortOptions pageSortOptions)
        {
            var accountHistory = GetHistory(clientId, accountFilters, pageSortOptions);            
            return PartialView("AccountHistoryWithPaging", accountHistory);
        }
    }
}