using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharper.SMS
{
    public static class EnumExtensions
    {
        public static int IntValue(this Enum enmValue)
        {
            return (int)enmValue.GetHashCode();
        }
    }
}
