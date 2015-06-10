using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Csharper.OliverTwist.GateService;
using Csharper.OliverTwist.Properties;
using Csharper.OliverTwist.Model;
using Csharper.Common;
using Csharper.OliverTwist.Repo;
using Csharper.OliverTwist.ServiceRepo;
using MvcFlan;
using OliverTwist.FilterContainers;

namespace Csharper.OliverTwist.Controllers
{
    public class StatisticsController : OTController
    {
        private StatisticsRepo _repo = null;
        private ClientRepo _clients = null;

        private StatisticsRepo Repo
        {
            get
            {
                if (_repo == null)
                    _repo = new StatisticsRepo(OTSession.LoginedUserName, OTSession.RealClient.Id, OTSession.OperationalClient.Id);
                return _repo;                
            }
        }

        private ClientRepo ClientRepo
        {
            get
            {
                if (_clients == null)
                    _clients = RepoGetter<ClientRepo>.Get(OTSession.LoginedUserName, OTSession.RealClient.Id, OTSession.OperationalClient.Id);
                return _clients;
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index(StatisticsFilterContainer filters, int? page, int? pageSize)
        {
            if (filters.ClientId.HasValue)
            {
                if (!ClientRepo.GetClients().Any(item => item.Id == filters.ClientId.Value) && !(filters.ClientId.Value == OTSession.OperationalClient.Id.Value))
                {
                    return new HttpUnauthorizedResult();
                }
            }
            if (filters.StartDate == DateTime.MinValue)
                filters.StartDate = null;
            if (filters.EndDate == DateTime.MinValue)
                filters.EndDate = null;

            return View(
                new SimpleContainerModel<StatisticsModel, StatisticsFilterContainer>()
                {
                    FilterContainer = filters,
                    Model = Repo.FillModel(pageSize??20, filters.ClientId, filters.UserId, filters.StartDate, filters.EndDate, page),
                    GridSortOptions = null
                }
                );
        }
    }
}
