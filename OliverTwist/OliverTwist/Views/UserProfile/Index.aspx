<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.UserProfileModel, OliverTwist.FilterContainers.UserProfilesFilterContainer>>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
	Ваши пользователи
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Ваши пользователи</h2>
    <% 
        Html.RenderPartial("UserProfileSearchFilter", Model.FilterContainer); 
    %>
    <% Html.RenderPartial("SearchResultsWithPaging", Model); %>          
    <hr />
    <div>
     <%= Html.ActionLink("Создать пользователя", "CreateNew") %>
    </div>
</asp:Content>
