using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Csharper.OliverTwist.Model;
using Csharper.Common;
using Csharper.OliverTwist.Repo;
using Csharper.OliverTwist.GateService;
using Csharper.OliverTwist.Properties;
using Csharper.OliverTwist.Services;

namespace Csharper.OliverTwist.ServiceRepo
{
    public class StatisticsRepo
    {
        private static ADEServiceClient _serviceClient = new ADEServiceClient();

        ClientRepo _clients = null;

        private string _loginedName;
        private long? _realClientId = null;
        private long? _operationaId = null;

        private ClientRepo Clients
        {
            get
            {
                if (_clients == null)
                    _clients = RepoGetter<ClientRepo>.Get(_loginedName, _realClientId , _operationaId);
                return _clients;
            }
        }

        public StatisticsRepo(string loginedName, long? realClientId, long? operationalClientId)
        {
            _loginedName = loginedName;
            _realClientId = realClientId;
            _operationaId = operationalClientId;
        }
        
        
        public StatisticsModel FillModel(long? rowsPerPage, long? clientId = null, Guid? userId = null, DateTime? dateStart = null, DateTime? dateEnd = null, long? pageNumber = null)
        {
            StatisticsModel result = new StatisticsModel()
            {
                Counters = new List<SMSCounter>(),
                Details = new List<SMSDetail>()
            };
            if (!clientId.HasValue)
                clientId = _operationaId;

            string sessionKey = _serviceClient.Login(Settings.Default.GateUserName, Settings.Default.GatePassword, Settings.Default.DefaultSenderName);
            Client client = Clients.GetClient(clientId.Value);
            if (client != null)
            {
                Dictionary<string, string> adittionalParams = null;
                if (userId.HasValue)
                {
                    adittionalParams = new Dictionary<string, string>();
                    adittionalParams.Add(ADEService.USER_ID, userId.Value.ToString());
                }
                result.Counters.AddRange(
                      _serviceClient.GetSMSCounters(
                            sessionKey, null, clientId.Value.ToString(),
                                null, dateStart, dateEnd, adittionalParams));

                    result.Details.AddRange(
                        _serviceClient.GetSMSDetalization(sessionKey,
                            clientId.Value.ToString(),
                                null, null, null, dateStart, dateEnd, rowsPerPage, pageNumber, adittionalParams));
                result.Paging = new SPPaginator(
                    result.Details, 
                    (int)(pageNumber??1),
                    (int)(rowsPerPage));
            }
            return result;
        }
    }
}