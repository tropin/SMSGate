using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Csharper.Common
{
    /// <summary>
    /// Статус сообщения
    /// </summary>
    [DataContract]
    public enum SMSStatus
    {
        /// <summary>
        /// Cтоит в очереди
        /// </summary>
        [Display(Name = "Cтоит в очереди")]
        [EnumMember]
        InQueue = 0,
        /// <summary>
        /// Отправлен поставщику
        /// </summary>
        [Display(Name = "Отправлен поставщику")]
        [EnumMember]
        Send = 1,
        /// <summary>
        /// Доставлен поставщиком
        /// </summary>
        [Display(Name = "Доставлен поставщиком")]
        [EnumMember]
        Delivered = 2,
        /// <summary>
        /// Ошибка отправки
        /// </summary>
        [Display(Name = "Ошибка отправки")]
        [EnumMember]
        SendError = 3,
        /// <summary>
        /// Ошибка валидации
        /// </summary>
        [Display(Name = "Ошибка валидации")]
        [EnumMember]
        ValidationError = 4,
        /// <summary>
        /// Отменена
        /// </summary>
        [Display(Name = "Отменена")]
        [EnumMember]
        Cancelled = 5,
        /// <summary>
        /// Попытка отмены
        /// </summary>
        [Display(Name = "Попытка отмены")]
        [EnumMember]
        CancellationPending = 6
    }
}
