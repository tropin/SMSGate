<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.UserProfileModel, OliverTwist.FilterContainers.UserProfilesFilterContainer>>" %>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcFlan" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%= Html.Grid(Model.PagedList)
    .WithModel(
                new MetaDataGridModel<Csharper.OliverTwist.Model.UserProfileModel>((column, renderWhen) =>
            {
                switch (renderWhen)
                {
                    case ColumnRender.Before:
                        column.For(
                            a => MvcHtmlString.Create(
                                string.Concat( 
                                    Html.ActionLink("Подробнее", "Details", new { id = a.UserId }),
                                    Html.ActionLink(" ", "Index", "Statistics", new {@ClientId=a.ClientId, @UserId = a.UserId}, new {@class = "sentHistory popupButton", @target="_blank", @title = "История отправки"})
                            ))).Encode(false);
                        break;
                    case ColumnRender.After:
                        column.For(a => !a.Roles.Contains("Admin") ? Ajax.ActionLink("Удалить", "DeleteUser", new { options = new JavaScriptSerializer().Serialize(new { Filter = Model.FilterContainer, Sort = Model.GridSortOptions }), page = Model.PagedList.PageNumber, id = a.UserId }, new AjaxOptions() { Confirm = string.Format("Удалить пользователя {0}?", a.Login), InsertionMode = InsertionMode.Replace, UpdateTargetId = "found_users" }) :
                            MvcHtmlString.Empty).Encode(false);
                        break;
                }
            }
        , Html)
    )
    .Sort(Model.GridSortOptions)
    .Attributes(@class => "table-list")
    .Empty("Нет пользователей соответствующих заданному фильтру")
    .RowAlternateColor()
%>