<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.ClientModel, OliverTwist.FilterContainers.ClientsFilterContainer>>" %>
<div id="found_clients">
    <% Html.RenderPartial("PagerUserControl", Model.PagedList); %>
    <% Html.RenderPartial("SearchResults", Model); %>
    <% Html.RenderPartial("PagerUserControl", Model.PagedList); %>
</div>

