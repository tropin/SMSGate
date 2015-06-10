using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using RoaminSMPP.Packet.Request;
using RoaminSMPP.Packet;

namespace Csharper.SenderService
{
    [DataContract]
    public class ProviderConfiguration
    {
        /// <summary>
        /// Доступные режимы подключения к шлюзу
        /// </summary>
        [DataMember]
        public List<SmppBind.BindingType> BindingTypes
        {
            get;
            set;
        }
        
        /// <summary>
        /// Доступне режимы нумераций исходящего абонента
        /// </summary>
        [DataMember]
        public List<TonNpiPair> SourceNumberings
        {
            get;
            set;
        }

        /// <summary>
        /// Доступне режимы нумераций принимаюещго абонента
        /// </summary>
        [DataMember]
        public List<TonNpiPair> DestinationNumberings
        {
            get;
            set;
        }

        /// <summary>
        /// Поддерживаемые версии протокола SMPP
        /// </summary>
        [DataMember]
        public List<Pdu.SmppVersionType> SupportedSMPPVersions
        {
            get;
            set;
        }

        /// <summary>
        /// Тип пейлоада
        /// </summary>
        [DataMember]
        public Pdu.PayloadTypeType PayloadType
        {
            get;
            set;
        }

        /// <summary>
        /// Время уведомления о жизни соединения
        /// </summary>
        [DataMember]
        public TimeSpan EnqureLinkInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Логин к поставщику
        /// </summary>
        [DataMember]
        public string SystemId
        {
            get;
            set;
        }

        /// <summary>
        /// Пароль к поставщику
        /// </summary>
        [DataMember]
        public string Password
        {
            get;
            set;
        }

        [DataMember]
        public string Host
        {
            get;
            set;
        }

        [DataMember]
        public int Port
        {
            get;
            set;
        }

        [DataMember]
        public string SystemType
        {
            get;
            set;
        }

        [DataMember]
        public string AddressRange
        {
            get;
            set;
        }

        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        [DataMember]
        public bool Support7Bit
        {
            get;
            set;
        }

        [DataMember]
        public bool Need7BitPacking
        {
            get;
            set;
        }

        [DataMember]
        public int TimeShift
        {
            get;
            set;
        }
    }
}
