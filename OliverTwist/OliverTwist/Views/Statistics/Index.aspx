<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SimpleContainerModel<Csharper.OliverTwist.Model.StatisticsModel, OliverTwist.FilterContainers.StatisticsFilterContainer>>" %>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcFlan" %>

<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
	Ваша статистика
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Ваша статистика</h2>
       <% Html.RenderPartial("StatisticsSearchFilter", Model.FilterContainer); %>
       <% Html.RenderPartial("SearchResultsWithPaging", Model); %>
</asp:Content>
