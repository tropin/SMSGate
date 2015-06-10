using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.Common
{
    public interface ISession
    {
        /// <summary>
        /// Время последнего обращения к сессии.
        /// </summary>
        DateTime LastAccess { get; set; }

        /// <summary>
        /// Ключ сессии
        /// </summary>
        string SessionKey { get; set; }
    }
}
