using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoaminSMPP.Packet.Response;
using RoaminSMPP.Packet.Request;

namespace Csharper.SMS.Storage
{
    public class DataBaseStorageProvider: ISMSStorageProvider
    {

        #region ISMSStorageProvider Members

        public SmppSubmitSmResp StoreMessage(SmppSubmitSm packet)
        {
            throw new NotImplementedException();
        }

        public SmppDataSmResp StoreMessage(SmppDataSm packet)
        {
            throw new NotImplementedException();
        }

        public SmppSubmitMultiResp StoreMessage(SmppSubmitMulti packet)
        {
            throw new NotImplementedException();
        }

        public SmppReplaceSmResp ReplaceMessage(SmppReplaceSm packet)
        {
            throw new NotImplementedException();
        }

        public SmppQuerySmResp QueryMessage(SmppQuerySm packet)
        {
            throw new NotImplementedException();
        }

        public SmppDeliverSmResp StoreIncomingMessage(SmppDeliverSm packet)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
