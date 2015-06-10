<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Csharper.OliverTwist.Model.StatisticsModel>" %>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcFlan" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
  <%= Html.Grid(Model.Counters)
        .WithModel(
                new MetaDataGridModel<Csharper.Common.SMSCounter>((column, renderWhen) => {} , Html)
        )
        .Attributes(@class => "table-list")
        .Empty("Нет счетчиков для клиента/пользователя")
        .RowAlternateColor()
    %>
    <br />
    <%= Html.Grid(Model.Details)
        .WithModel(
                new MetaDataGridModel<Csharper.Common.SMSDetail>((column, renderWhen) => {} , Html)
        )
        .Attributes(@class => "table-list")
        .Empty("Нет детализации для клиента/пользователя")
        .RowAlternateColor()
    %>