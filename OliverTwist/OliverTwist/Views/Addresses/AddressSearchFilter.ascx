<%@ Control Language="C#" 
    Inherits="System.Web.Mvc.ViewUserControl<OliverTwist.FilterContainers.AddressFilterContainer>" 
%>
<%@ Import Namespace="MvcContrib.UI" %>
<%using (Html.BeginForm("Index", "Addresses", FormMethod.Get, new { id = "addressSearch" }))
  { %>
<div id="searchFilter">
    <% Html.Hidden("SelectedGroupId"); %>
    <div>
        <label>
            Фамилия
        </label>
        <%:Html.TextBox("lastName", Model.LastName, new { size = 30, maxlength = "40" })%>
    </div>
    <div>
        <label>
            Номер мобильного телефона
        </label>
        <%:Html.TextBox("mobilePhone", Model.MobilePhone, new { size = 30, maxlength = "40" })%>
    </div>
    <div>
        <label>
            Город
        </label>
        <%:Html.EditorFor(model => model.City)%>
    </div>
    <div>
        <label>
            Пол
        </label>
        <%:Html.EditorFor(model => model.Sex)%>
    </div>
    <span>
        <input type="submit" value="Искать" class="btnNeutral" />
    </span>
</div>
<%} %>