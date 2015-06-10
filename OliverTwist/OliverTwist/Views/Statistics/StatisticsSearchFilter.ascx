<%@ Control Language="C#" 
    Inherits="System.Web.Mvc.ViewUserControl<OliverTwist.FilterContainers.StatisticsFilterContainer>" 
%>
<%@ Import Namespace="MvcContrib.UI" %>
<%using (Html.BeginForm("Index", "Statistics", FormMethod.Get, new { id = "statisticsFilter" }))
  {
  %>
<div id="searchFilter">
    <div>
        <label>
            С даты:
        </label>
        <%:Html.EditorFor(model => model.StartDate)%>
    </div>
    <div>
        <label>
            По дату:
        </label>
        <%:Html.EditorFor( model => model.EndDate)%>
    </div>
    <%: Html.HiddenFor(model=>model.UserId) %>
    <%: Html.HiddenFor(model=>model.ClientId) %>
    <span>
        <input type="submit" value="Искать" class="btnNeutral" />
    </span>
</div>
<%} %>