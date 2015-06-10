<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="OliverTwist.Converters" %>
<script runat="server">
    private static TimeZoneConverter tz = new TimeZoneConverter();
    
    public string ModelValue
    {
        get
        {
            var res = tz.ConvertFrom(ViewData.Model);
            return res == null ? string.Empty : res.ToString();
        }
    }
    
    
    
    //if (modelMetadata.AdditionalValues.ContainsKey(Globals.TypeConverterAttributeKey))
    //{
    //    TypeConverterAttribute attr = (TypeConverterAttribute)modelMetadata.AdditionalValues[Globals.TypeConverterAttributeKey];
    //    TypeConverter cnv = (TypeConverter)Activator.CreateInstance(Type.GetType(attr.ConverterTypeName));
    //    MethodInfo mi = cnv.GetType().GetMethod("ConvertFrom", new Type[] { typeof(object) });
    //    expression = Expression.Call(Expression.Constant(cnv), mi, expression);
    //    expression = Expression.Convert(expression, modelMetadata.ModelType);
    //}
</script>
<%: Html.Label(String.IsNullOrEmpty(ViewData.ModelMetadata.DisplayFormatString) ? ModelValue : string.Format(ViewData.ModelMetadata.DisplayFormatString, ModelValue))%>

