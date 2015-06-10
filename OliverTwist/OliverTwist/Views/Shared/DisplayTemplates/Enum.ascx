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
<%: Html.Label(ModelValue == null? "----": ModelValue.GetDisplayName()) %>