using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csharper.Common
{
    [DataContract]
    public class SMSCounter
    {
        /// <summary>
        /// Статус
        /// </summary>
        [DisplayName("Статус СМС")]
        [DataType("Enum")]
        [DataMember]        
        public SMSStatus Status { get; set; }
        /// <summary>
        /// Количество СМС в заданном статусе
        /// </summary>
        [DisplayName("Количество СМС в заданном статусе")]
        [DisplayFormat(DataFormatString="{0:##}")]
        [DataMember]
        public decimal Count { get; set; }
    }
}
