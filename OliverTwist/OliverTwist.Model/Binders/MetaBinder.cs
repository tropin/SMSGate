using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace Csharper.OliverTwist.Model.Binders
{
    public class MetaBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            if (propertyDescriptor.Converter != null && propertyDescriptor.PropertyType!=typeof(string))
            {
                TypeConverter converter = propertyDescriptor.Converter;
                ConverterContext cxt = new ConverterContext(propertyDescriptor);
                ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
                try
                {
                    return UnwrapPossibleArrayType(CultureInfo.CurrentCulture, valueProviderResult.RawValue, bindingContext.ModelType, converter, cxt);  
                }
                catch (Exception exception)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, exception);
                    return null;
                }
            }
            else
               return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }

        private static object UnwrapPossibleArrayType(CultureInfo culture, object value, Type destinationType, TypeConverter converter, ITypeDescriptorContext converterContext)
        {
            if ((value == null) || destinationType.IsInstanceOfType(value))
            {
                return value;
            }
            Array array = value as Array;
            if (destinationType.IsArray)
            {
                Type elementType = destinationType.GetElementType();
                if (array != null)
                {
                    IList list = Array.CreateInstance(elementType, array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        list[i] = ConvertSimpleType(culture, array.GetValue(i), elementType, converter, converterContext);
                    }
                    return list;
                }
                object singleResult = ConvertSimpleType(culture, value, elementType, converter, converterContext);
                IList list_single = Array.CreateInstance(elementType, 1);
                list_single[0] = singleResult;
                return list_single;
            }
            if (array == null)
            {
                return ConvertSimpleType(culture, value, destinationType, converter, converterContext);
            }
            if (array.Length > 0)
            {
                value = array.GetValue(0);
                return ConvertSimpleType(culture, value, destinationType, converter, converterContext);
            }
            return null;
        }

        private static object ConvertSimpleType(CultureInfo culture, object value, Type destinationType, TypeConverter converter, ITypeDescriptorContext converterContext)
        {
            if (converter.CanConvertFrom(value.GetType()))
            {
                return converter.ConvertFrom(converterContext, culture, value);
            }
            else throw new Exception(string.Format("Cannot find convertor for this conversion type {0} -> {1}", value.GetType().FullName, destinationType.FullName));
        }
    }
}
