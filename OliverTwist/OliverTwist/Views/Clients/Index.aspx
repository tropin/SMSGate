<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.ClientModel, OliverTwist.FilterContainers.ClientsFilterContainer>>" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
	Ваши Клиенты
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Ваши клиенты</h2>
    <% Html.RenderPartial("ClientSearchFilter", Model.FilterContainer); %>
    <% Html.RenderPartial("SearchResultsWithPaging", Model); %>          
    <hr />
    <div>
     <%= Html.ActionLink("Создать клиента", "CreateNew") %>
    </div>
</asp:Content>
