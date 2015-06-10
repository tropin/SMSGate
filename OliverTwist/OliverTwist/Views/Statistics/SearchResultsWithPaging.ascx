<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.SimpleContainerModel<Csharper.OliverTwist.Model.StatisticsModel, OliverTwist.FilterContainers.StatisticsFilterContainer>>" %>
<div id="found_clients">
    <% Html.RenderPartial("PagerUserControl", Model.Model.Paging); %>
    <% Html.RenderPartial("SearchResults", Model.Model); %>
    <% Html.RenderPartial("PagerUserControl", Model.Model.Paging); %>
</div>

