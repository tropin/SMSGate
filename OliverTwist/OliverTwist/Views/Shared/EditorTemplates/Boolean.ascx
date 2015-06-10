<%@ control language="C#" inherits="System.Web.Mvc.ViewUserControl" %>

<script runat="server">
	private List<SelectListItem> TriStateValues
	{
		get
		{
			return new List<SelectListItem> {
				new SelectListItem { Text = "----",
									 Value = String.Empty,
									 Selected = !Value.HasValue },
				new SelectListItem { Text = "Да",
									 Value = "true",
									 Selected = Value.HasValue && Value.Value },
				new SelectListItem { Text = "Нет",
									 Value = "false",
									 Selected = Value.HasValue && !Value.Value },
			};
		}
	}
	private bool? Value
	{
		get
		{
			bool? value = null;
			if (ViewData.Model != null)
			{
				value = Convert.ToBoolean(ViewData.Model,
							System.Globalization.CultureInfo.InvariantCulture);
			}
			return value;
		}
	}
</script>

<% if (ViewData.ModelMetadata.IsNullableValueType) { %>
	<%= Html.DropDownList(string.Empty, TriStateValues, new { @class = "list-box tri-state" })%>
<% } else { %>
	<%= Html.CheckBox(string.Empty, Value ?? false, new { @class = "check-box" })%>
<% } %>