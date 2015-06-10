using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Csharper.Common.Services
{
    [ServiceContract]
    public interface IGatewayCallback
    {
        [OperationContract]
        bool NotifyDelivered(Guid smsId, string clientId, string distibutionId = null, Dictionary<string,string> additionalParams = null);
        
        [OperationContract]
        bool NotifyFailed(Guid smsId, string clientId, string distibutionId = null, Dictionary<string, string> additionalParams = null);
    }
}
