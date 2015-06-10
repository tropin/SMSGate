using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Csharper.Common
{
    [DataContract]
    public struct SMSSeriesId
    {
        private static SMSSeriesId _empty = new SMSSeriesId()
        {
            Id = Guid.Empty,
            ExternalId = string.Empty,
            SeriesPosition = 0
        };

        [DataMember]
        public Guid Id {get; set;}
        [DataMember]
        public string ExternalId { get; set; }
        [DataMember]
        public int SeriesPosition { get; set; }

        public static SMSSeriesId Empty
        {
            get
            {
                return _empty; 
            }
        }

    }
}
