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

namespace OliverTwist.Controllers
{
    [HandleError]
    public class AddressesController : OTController
    {
        private IAddressRepo _addressRepo;

        private IAddressRepo AddressRepo
        {
            get
            {       
                return _addressRepo;
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (OTSession.OperationalClient != null)
            {
                _addressRepo = RepoGetter<AddressRepo>.Get(OTSession.LoginedUserName, OTSession.RealClient.Id, OTSession.OperationalClient.Id);
            }
        }

        [Authorize]
        public JsonResult GetGroups(long? id)
        {
            return Json(AddressRepo.GetGroups(id ?? 0));
        }
        
        
        [Authorize]
        public PartialViewResult DeleteAddress(string options, int page, long id)
        {
            AddressRepo.DeleteAddress(id);
            OptionsHolder<AddressModel> holder = OptionsHolder<AddressModel>.GetHolder(options); 
            PageSortOptions pso = new PageSortOptions()
            {
                Column = holder.Sort.Column,
                Direction = holder.Sort.Direction,
                Page = page
            };
            return PartialView("SearchResultsWithPaging", GetAddressList(holder.Filter, pso));
        }

        [Authorize]
        private ListContainerModel<AddressModel, AddressFilterContainer> GetAddressList(AddressModel addressFilters, PageSortOptions pageSortOptions)
        {
            var addressPagedList = AddressRepo.GetAdressesProjected().AsFiltered(addressFilters).AsPagination(pageSortOptions);
            var addressViewFilterContainer = new AddressFilterContainer()
                {
                    LastName = addressFilters.LastName,
                    MobilePhone = addressFilters.MobilePhone,
                    City = addressFilters.City,
                    Sex = addressFilters.Sex
                };
            
            var gridSortOptions = new GridSortOptions 
            { 
                Column = pageSortOptions.Column, 
                Direction = pageSortOptions.Direction 
            };

            var clientListContainer = new ListContainerModel<AddressModel, AddressFilterContainer>() 
            {
                PagedList = addressPagedList,
                FilterContainer = addressViewFilterContainer,
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
        public ActionResult Index(AddressModel clientFilters, PageSortOptions pageSortOptions)
        {
            return View(GetAddressList(clientFilters, pageSortOptions));
        }
        [Authorize]
        public ActionResult Details(long id)
        {
            return View(AddressRepo.GetAdressesProjected(id));
        }
        
        [Authorize]
        public ActionResult CreateNew()
        {
            return View("Details", new AddressModel());
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult CreateNew(AddressModel model)
        {
            return Details(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Details(AddressModel model)
        {
            if (ModelState.IsValid)
            {
                AddressRepo.SaveAddress(model);
                return RedirectToAction("Index");
            }
            // Если что-то не сошлось пепоказываем форму
            return View("Details", model);
        }
    }
}
