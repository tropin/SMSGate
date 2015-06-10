using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Csharper.Common.Services;
using Csharper.OliverTwist.Repo;
using Csharper.OliverTwist.Model;
using Csharper.OliverTwist.Model.Billing;

namespace Csharper.OliverTwist.Services
{
    public class GatewayCallback : IGatewayCallback
    {
        
        #region IGatewayCallback Members

        public bool NotifyDelivered(Guid smsId, string clientId, string distibutionId = null, Dictionary<string,string> additionalParams = null)
        {
            long intClientId;
            bool result = false;
            if (long.TryParse(clientId, out intClientId))
            {
                ClientModel client = ClientRepo.GetClientConcrete(intClientId);
                if (client != null && client.Id.HasValue)
                {
                    if (client.DebtingType == DebtingType.ByDelivered)
                    {
                        if (!additionalParams.ContainsKey(ADEService.EXTERNAL))
                        {
                            long? intDistibutionId = null;
                            if (!string.IsNullOrEmpty(distibutionId))
                            {
                                long dId;
                                if (long.TryParse(distibutionId, out dId))
                                    intDistibutionId = dId;
                            }
                            result = BillingProcessor.CommintSMSSend(intClientId, distributionId: intDistibutionId);
                        }
                        else
                        {
                            result = BillingProcessor.CommintSMSSend(intClientId, extDistributionId: distibutionId);
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool NotifyFailed(Guid smsId, string clientId, string distibutionId = null, Dictionary<string, string> additionalParams = null)
        {
            long intClientId;
            bool result = false;
            if (long.TryParse(clientId, out intClientId))
            {
                ClientModel client = ClientRepo.GetClientConcrete(intClientId);
                if (client != null && client.Id.HasValue)
                {
                    if (client.DebtingType == DebtingType.ByDelivered)
                    {
                        if (!additionalParams.ContainsKey(ADEService.EXTERNAL))
                        {
                            long? intDistibutionId = null;
                            if (!string.IsNullOrEmpty(distibutionId))
                            {
                                long dId;
                                if (long.TryParse(distibutionId, out dId))
                                    intDistibutionId = dId;
                            }
                            result = BillingProcessor.ResetSMSSend(intClientId, distributionId: intDistibutionId);
                        }
                        else
                        {
                            result = BillingProcessor.ResetSMSSend(intClientId, extDistributionId: distibutionId);
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
