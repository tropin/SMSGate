<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.AddressModel, OliverTwist.FilterContainers.AddressFilterContainer>>" %>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcFlan" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%= Html.Grid(Model.PagedList)
    .WithModel(
            new MetaDataGridModel<Csharper.OliverTwist.Model.AddressModel>((column, renderWhen) =>
            {
                if (renderWhen == ColumnRender.Before)
                {
                    column.For(a => Html.ActionLink("Подробнее", "Details", new { id = a.Id })).Encode(false);
                }
                else
                {
                    column.For(a => Ajax.ActionLink("Удалить", "DeleteAddress", new { options = new JavaScriptSerializer().Serialize(new { Filter = Model.FilterContainer, Sort = Model.GridSortOptions }), page = Model.PagedList.PageNumber, id = a.Id }, new AjaxOptions() { Confirm = string.Format("Удалить адрес {0} {1}?", a.LastName, a.MobilePhone), InsertionMode = InsertionMode.Replace, UpdateTargetId = "found_addresses" })).Encode(false);
                }
            }
        , Html)
    )
    .Sort(Model.GridSortOptions)
    .Attributes(@class => "table-list")
    .Empty("Нет адресов соответствующих заданному фильтру")
    .RowAlternateColor()
%>