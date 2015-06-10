using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.Common;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Csharper.Common
{
    [DataContract]
    public class SMSCheckItem: IPagedResult
    {
        /// <summary>
        /// Уникальный номер СМС в системе
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// Номер сообщения в удаленной системе
        /// </summary>
        [DataMember]
        public string MessageId { get; set; }
        /// <summary>
        /// Статус СМС
        /// </summary>
        [DataMember]
        public SMSStatus Status {get; set;}

        /// <summary>
        /// Дата обновления статуса
        /// </summary>
        [DataMember]
        public DateTime LastCheckUTC { get; set; }

        #region IPagedResult Members

        [ScaffoldColumn(false)]
        [DataMember]
        public long? Row { get; set; }

        [ScaffoldColumn(false)]
        [DataMember]
        public int? Records { get; set; }

        #endregion
    }
}
