<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.ClientAccountActionModel, OliverTwist.FilterContainers.AccountHistoryFilterContainer>>" %>
 <% Html.RenderPartial("PagerUserControl", Model.PagedList); %>
 <% Html.RenderPartial("AccountHistory", Model); %>
 <% Html.RenderPartial("PagerUserControl", Model.PagedList); %>

