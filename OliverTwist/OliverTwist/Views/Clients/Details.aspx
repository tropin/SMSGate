<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Csharper.OliverTwist.Model.ClientModel>" %>
<%@ Import Namespace="OliverTwist.FilterContainers" %>
<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
	Клиент "<%: 
	            MvcHtmlString.Create(string.Format("Клиент: {0}", string.IsNullOrEmpty(Model.OrganizationName)? "[Новый]" : Model.OrganizationName)) 
	        %>"
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Информация о клиенте</h2>
     <div id="searchFilter">
        <%: Html.ActionLink("« Вернуться к списку", "Index", null, new { style = "font-weight:bold" })%>
    </div>
    <%using(Html.BeginForm()){%> 
    <div id="detailsContainer">
        <%: Html.EditorForModel() %>
    </div>
        <%: Html.EditorFor(model=>model.CostRanges) %>
    <span>
        <input type="submit" value="<%= 
            Model.Id==null?"Создать":"Изменить" 
        %>" class="btnNeutral" />
    </span>
    <% }%>
    <p />
</asp:Content>
