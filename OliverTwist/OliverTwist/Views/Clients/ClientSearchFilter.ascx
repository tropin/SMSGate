<%@ Control Language="C#" 
    Inherits="System.Web.Mvc.ViewUserControl<OliverTwist.FilterContainers.ClientsFilterContainer>" 
%>
<%@ Import Namespace="MvcContrib.UI" %>
<%using (Html.BeginForm("Index", "Clients", FormMethod.Get, new { id = "clientSearch" }))
  { %>
<div id="searchFilter">
    <div>
        <label>
            Логин
        </label>
        <%:Html.TextBox("login", Model.Login, new { size = 30, maxlength = "40" })%>
    </div>
    <div>
        <label>
            Наименование организации
        </label>
        <%:Html.TextBox("organizationName", Model.OrganizationName, new { size = 30, maxlength = "40" })%>
    </div>
    <div>
        <label>
            Статус пользователя
        </label>
        <%:Html.EditorFor(model => model.Status)%>
    </div>
    <div>
        <label>
            Диллер?
        </label>
        <%:Html.EditorFor(model => model.IsDealler)%>
    </div>
    <span>
        <input type="submit" value="Искать" class="btnNeutral" />
    </span>
</div>
<%} %>