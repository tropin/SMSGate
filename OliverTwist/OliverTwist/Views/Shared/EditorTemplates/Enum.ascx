<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="MvcFlan" %>
<script runat="server">
	private Enum ModelValue
	{
		get
		{
            Enum value = null;
			if (ViewData.Model != null)
			{
				value = (Enum)ViewData.Model;
			}
			return value;
		}
	}
</script>
<%: 
    Html.DropDownList
    (
        string.Empty,
        ModelValue.ToSelectList
            (
                enumType: ViewData.ModelMetadata.IsNullableValueType?
                          ViewData.ModelMetadata.ModelType.GetGenericArguments()[0]   
                :ViewData.ModelMetadata.ModelType
            ).ToList(),
            ViewData.ModelMetadata.IsNullableValueType? "-- Не указано --": null
    ) 
%>