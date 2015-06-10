using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace OliverTwist.Converters
{
    public class ListOfStringConverter : TypeConverter
    {        
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;
            if (value is List<string> && destinationType == typeof(string))
            {
                return string.Join(",",(value as List<string>));
            }
            else
                return null;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;
            
            if (value.GetType() == typeof(string))
            {
                return new List<string>((value as string).Split(new[]{";",","}, StringSplitOptions.RemoveEmptyEntries));
            }
            else
                return null;       
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return false;
        }
    }
}