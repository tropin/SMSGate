using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Группа адресов
    /// </summary>
    public class AddressGroupModel
    {
        /// <summary>
        /// Имя группы
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// Аттрибуты группы
        /// </summary>
        public object attr { get; set; }
        /// <summary>
        /// Состояние при отрисовке
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// Непосредственные потомки
        /// </summary>
        public List<AddressGroupModel> children { get; set; }
        public AddressGroupModel() {}
    }
}
