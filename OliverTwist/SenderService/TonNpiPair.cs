using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoaminSMPP.Packet;
using System.Runtime.Serialization;

namespace Csharper.SenderService
{
    /// <summary>
    /// Пара, определяющая тип адресата
    /// </summary>
    [DataContract]
    public class TonNpiPair
    {
        /// <summary>
        /// Тип номера
        /// </summary>
        [DataMember]
        public Pdu.TonType Ton {get; set;}
        /// <summary>
        /// План нумерации
        /// </summary>
        [DataMember]
        public Pdu.NpiType Npi {get; set;}

        public TonNpiPair(Pdu.TonType ton, Pdu.NpiType npi)
        {
            Ton = ton;
            Npi = npi;
        }
    }
}
