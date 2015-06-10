<%@ Control Language="C#" 
    Inherits="System.Web.Mvc.ViewUserControl<OliverTwist.FilterContainers.UserProfilesFilterContainer>" 
%>
<%@ Import Namespace="MvcContrib.UI" %>
<%using (Html.BeginForm("Index", "UserProfile", FormMethod.Get, new { id = "userSearch" }))
  { %>
<div id="searchFilter">
    <%: Html.DisplayForModel("FilterContainer") %>
    <span>
        <input type="submit" value="Искать" class="btnNeutral" />
    </span>
</div>
<%} %>