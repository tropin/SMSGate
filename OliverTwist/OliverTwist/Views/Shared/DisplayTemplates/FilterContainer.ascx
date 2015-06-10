<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% if (ViewData.TemplateInfo.TemplateDepth > 1) { %>
	<%= ViewData.ModelMetadata.SimpleDisplayText%>
<% }
	else { %>    
	<% foreach (var prop in ViewData.ModelMetadata.Properties.Where(pm => pm.ShowForEdit
					&& !ViewData.TemplateInfo.Visited(pm))) { %>
        <div>
		<% if (prop.HideSurroundingHtml) { %>
			<%= Html.Editor(prop.PropertyName) %>
		<% } else { %>
			<% if (!String.IsNullOrEmpty(Html.Label(prop.PropertyName).ToHtmlString())) { %>
				<%= Html.Label(prop.PropertyName) %>
			<% } %>
				<%= 
				    Html.Editor(prop.PropertyName) 
				%>
		<% } %>
        </div>
	<% } %>
<% } %>

