using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Csharper.OliverTwist.Model
{
    public class NewClientModel : UserProfileModel
    {
        /// <summary>
        /// Наименование организации
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Наименование организации является обязательным")]
        [DisplayName("Наименование организации")]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Имя пользователя пустое")]
        [DisplayName("Имя пользователя")]
        public string UserName { get; set; }

        /// <summary>
        /// Статус клиента
        /// </summary>
        [DataType("Enum")]
        [DisplayName("Статус клиента")]
        public ClientStatus Status { get; set; }

        /// <summary>
        /// Диллер
        /// </summary>
        [DisplayName("Диллер")]
        public bool IsDealler { get; set; }
    }
}
