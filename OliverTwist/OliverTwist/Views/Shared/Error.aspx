<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Ошибка
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Дико извиняемся, но либо у вы запросили то чего у нас нет, либо то, чего мы дать не можем
    </h2>
     Полное сообщение об ошибке (в отладочных целях)<br />
    <%if (Model.Exception != null)
      { %>
    <%=Html.Encode(Model.Exception.Message)%>
    <%} %>
</asp:Content>
