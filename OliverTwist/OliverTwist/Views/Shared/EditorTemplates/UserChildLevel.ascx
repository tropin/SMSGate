<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="OliverTwist.FilterContainers" %>
<%: Html.DropDownList("", UserChildLevelDropdown.Get(ViewData.Model == null ? string.Empty : ViewData.Model.ToString()), "-- Мои и всех клиентов --")%>

