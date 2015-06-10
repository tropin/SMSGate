<%@ Control Language="C#" 
    Inherits="System.Web.Mvc.ViewUserControl<dynamic>" 
%>
<%@ Import Namespace="MvcContrib.UI" %>

<%
  using (Ajax.BeginForm("AccountHistorySearchIndex", "ClientAccounts", null,
      new AjaxOptions() { HttpMethod = "GET", InsertionMode = InsertionMode.Replace, UpdateTargetId = Model.ControlIdToRefresh, OnSuccess = "ExtendAjaxLinks" }, new { id = "accountHistorySearch" }))
  { 
%>
    <%: 
        InputExtensions.Hidden(Html,"clientId", Model.ClientId)
    %>
<div id="searchFilter">
    <div>
        <label>
            Дата операции
        </label>
        <%:InputExtensions.TextBox(Html, "versionDate", Model.Model.VersionDate, new { size = 20, maxlength = "20" })%>
    </div>
    <div>
        <label>
            Пользователь
        </label>
        <%:InputExtensions.TextBox(Html, "managerName", Model.Model.ManagerName, new { size = 20, maxlength = "20" })%>
    </div>

    <div>
        <label>
            Клиент
        </label>
        <%:InputExtensions.TextBox(Html, "realClientName", Model.Model.RealClientName, new { size = 20, maxlength = "20" })%>
    </div>
    <div>
        <label>
            Кому перечислено
        </label>
        <%:InputExtensions.TextBox(Html, "targetAccountOrganizationName", Model.Model.TargetAccountOrganizationName, new { size = 30, maxlength = "30" })%>
    </div>
    <div>
        <label>
            Внесенная сумма
        </label>
        <%:InputExtensions.TextBox(Html, "moneyVolume", Model.Model.MoneyVolume, new { size = 10, maxlength = "10" })%>
    </div>
    <div>
        <label>
            Стоимость одной СМС
        </label>
        <%:InputExtensions.TextBox(Html, "quickCost", Model.Model.QuickCost, new { size = 10, maxlength = "10" })%>
    </div>  
 
    <span>
        <input type="submit" value="Искать" class="btnNeutral" />
    </span>
</div>
<%} %>