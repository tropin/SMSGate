<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
    private Csharper.OliverTwist.Model.SessionAcessor _sAcc = null;

    public Csharper.OliverTwist.Model.SessionAcessor SessionAccessor
    {
        get
        {
            if (_sAcc == null)
                _sAcc = Csharper.OliverTwist.Model.SessionAcessor.GetAcessor();
            return _sAcc;
        }
    }
    
    public Csharper.OliverTwist.Model.ClientModel OperationalClient
    {
        get
        {
            return SessionAccessor.OperationalClient;
        }
    }
    
    public bool IsLoggedOn
    {
        get
        {
            return Request.IsAuthenticated &&
                   OperationalClient != null;
        }
    }
    
</script>

<%
    if (IsLoggedOn)
    {
%>
        Добро пожаловать <b><%: Page.User.Identity.Name %></b>!
        [ <%: Html.ActionLink("Выйти", "LogOff", "Account") %> ]
        <br />
        [ <%: Html.ActionLink("Профиль", "Details", "UserProfile", new { id = SessionAccessor.LoginedUserId }, null)%> ]
        <div>
            <span>Вы вошли за <%: SessionAccessor.RealClient.OrganizationName%></span>
            <% if (SessionAccessor.CurrentBallance >= 0)
               {%>
                <span>Доступно <b><%: SessionAccessor.CurrentBallance.ToString("##########") %></b> СМС</span>
            <% }
               else
               {%>
                <span>Задолженность <b><%: (-SessionAccessor.CurrentBallance).ToString("##########") %></b> СМС</span>
            <%} %>            
            
            <%if (SessionAccessor.IsClientSwitched) 
              { %>
               <br />
               <span>Вы действуете от имени: <%:OperationalClient.OrganizationName%></span>
                 <%:Html.ActionLink("Отменить", "RevertCurrent", "Clients")%>
            <%} %>
        </div>
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("Войти", "LogOn", "Account") %> ]
<%
    }
%>
