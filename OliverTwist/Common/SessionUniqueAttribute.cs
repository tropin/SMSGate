using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.Common
{
    /// <summary>
    /// Атрибут уникальности поля сессии
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SessionUniqueAttribute: Attribute
    {
    }
}
