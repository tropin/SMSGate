<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Csharper.OliverTwist.Model.AddressModel>" %>
<%@ Import Namespace="OliverTwist.FilterContainers" %>
<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
	Адрес "<%: 
	            MvcHtmlString.Create(string.Format("{0} {1}", string.IsNullOrEmpty(Model.LastName) ? "Ф." : Model.LastName, string.IsNullOrEmpty(Model.MobilePhone) ? "---" : Model.MobilePhone)) 
	        %>"
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Адрес доставки</h2>
     <div id="searchFilter">
        <%: Html.ActionLink("« Вернуться к списку", "Index", null, new { style = "font-weight:bold" })%>
    </div>
    <%using(Html.BeginForm()){%> 
    <div id="detailsContainer">
        <%: Html.EditorForModel() %>
    </div>
    <span>
        <input type="submit" value="<%= 
            Model.Id==null?"Создать":"Изменить" 
        %>" class="btnNeutral" />
    </span>
    <% }%>
    <p />
</asp:Content>
