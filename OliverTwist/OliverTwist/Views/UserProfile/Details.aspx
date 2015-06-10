<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Csharper.OliverTwist.Model.UserProfileModel>" %>
<%@ Import Namespace="OliverTwist.FilterContainers" %>
<asp:Content ID="titleContent" ContentPlaceHolderID="TitleContent" runat="server">
	Клиент "<%: 
	            MvcHtmlString.Create(string.Format("Пользователь: {0}", string.IsNullOrEmpty(Model.Login) ? "[Новый]" : Model.Login))
	        %>"
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Информация о пользователе</h2>
     <div id="searchFilter">
        <%: Html.ActionLink("« Вернуться к списку", "Index", null, new { style = "font-weight:bold" })%>
    </div>
    <%using(Html.BeginForm()){%> 
    <div id="detailsContainer">
        <%: Html.EditorForModel() %>
        <%: Html.Hidden("UserId", Model.UserId) %>
        <%: Html.Hidden("Id", Model.Id.ToString()) %>
    </div>
    <% if (Model.Id == Csharper.OliverTwist.Model.SessionAcessor.GetAcessor().RealClient.Id)
       {%>
       <div><%: Html.ActionLink("Сменить пароль", "ChangePassword", "Account", null, new { style = "font-weight:bold" })%></div>
    <% } %>
    <span>
        <input type="submit" value="<%= 
            Model.Id==null?"Создать":"Изменить" 
        %>" class="btnNeutral" />
    </span>
    <% }%>
    <p />
</asp:Content>
