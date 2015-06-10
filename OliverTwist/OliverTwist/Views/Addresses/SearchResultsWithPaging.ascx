<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.AddressModel, OliverTwist.FilterContainers.AddressFilterContainer>>" %>
<div id="found_addresses">
    <% Html.RenderPartial("PagerUserControl", Model.PagedList); %>
    <% Html.RenderPartial("SearchResults", Model); %>
    <% Html.RenderPartial("PagerUserControl", Model.PagedList); %>
</div>

