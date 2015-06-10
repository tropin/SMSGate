<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.UserProfileModel, OliverTwist.FilterContainers.UserProfilesFilterContainer>>" %>
<div id="found_users">
    <% Html.RenderPartial("PagerUserControl", Model.PagedList); %>
    <div style="overflow: scroll;">
    <% Html.RenderPartial("SearchResults", Model); %>
    </div>
    <% Html.RenderPartial("PagerUserControl", Model.PagedList); %>
</div>

