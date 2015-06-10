using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace OliverTwist.Converters
{
    public class JSONStringConverter : TypeConverter
    {
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        
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
            if (destinationType == typeof(string))
            {
                return serializer.Serialize(value);
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
                try
                {
                    return serializer.Deserialize((string)value, context.PropertyDescriptor.PropertyType);
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;       
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
    }
}