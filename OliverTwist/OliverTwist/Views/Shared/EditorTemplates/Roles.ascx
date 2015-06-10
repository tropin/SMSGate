<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="OliverTwist.FilterContainers" %>
<%: Html.DropDownList("", RolesDropdown.Get(ViewData.Model == null ? string.Empty : ViewData.Model.ToString()), "-- Не указано --")%>

