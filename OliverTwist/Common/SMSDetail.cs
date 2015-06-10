using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Csharper.Common
{
    [DataContract]
    public class SMSDetail: IPagedResult
    {
        /// <summary>
        /// Идентификатор СМС
        /// </summary>
        [DisplayName("Идентификатор СМС")]
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Отправитель
        /// </summary>
        [DisplayName("Отправитель")]
        [DataMember]
        public string Sender { get; set; }

        /// <summary>
        /// Время отправки
        /// </summary>
        [DisplayName("Время отправки")]
        [DataMember]
        public DateTime EnqueueTime { get; set; }
        /// <summary>
        /// Время актуализации статуса
        /// </summary>
        [DisplayName("Время актуализации статуса")]
        [DataMember]
        public DateTime StatusTime { get; set; }
        /// <summary>
        /// Статус СМС
        /// </summary>
        [DisplayName("Статус СМС")]
        [DataType("Enum")]
        [DataMember]
        public SMSStatus Status { get; set; }
        /// <summary>
        /// Получатель
        /// </summary>
        [DisplayName("Получатель")]
        [DataMember]
        public string Destination { get; set; }
        /// <summary>
        /// Текст СМС
        /// </summary>
        [DisplayName("Текст СМС")]
        [DataMember]
        public string Text { get; set; }

        #region IPagedResult Members

        [ScaffoldColumn(false)]
        [DataMember]
        public long? Row {get; set;}

        [ScaffoldColumn(false)]
        [DataMember]
        public int? Records {get; set;}

        #endregion
    }
}
