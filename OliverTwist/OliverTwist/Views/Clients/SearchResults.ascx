<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.ClientModel, OliverTwist.FilterContainers.ClientsFilterContainer>>" %>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcFlan" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<script runat="server">
    public MvcHtmlString RenderAccount(Csharper.OliverTwist.Model.ClientModel model)
    {
        MvcHtmlString result = MvcHtmlString.Empty;
        if (model.Account.HasValue)
                            {
                                result = MvcHtmlString.Create(string.Concat(
                                    Html.GetLinkLabel("popuplink", "ChangeClientAccount", model.Account.ToString(), "ClientAccounts", new { clientId = model.Id },
                                    new { wndtitle = string.Format("Изменение счета клиента {0}", model.OrganizationName), linkclass = "changeAccount" }).ToHtmlString(),
                                    Html.GetLinkLabel("popuplink", "AccountHistoryIndex", string.Empty, "ClientAccounts", new { clientId = model.Id },
                                    new { wndtitle = string.Format("История счета клиента {0}", model.OrganizationName), linkclass = "historyOfClientAccount", popwidth = 900 }).ToHtmlString(),
                                    Html.ActionLink(" ", "Index", "Statistics", new {@ClientId = model.Id}, new {@class = "sentHistory popupButton", @target="_blank", @title = "История отправки"})
                                    ));
                            }
        return result;
    }

</script>
<%= Html.Grid(Model.PagedList)
    .WithModel(
            new MetaDataGridModel<Csharper.OliverTwist.Model.ClientModel>((column, renderWhen) =>
            {
                switch (renderWhen)
                {
                    case ColumnRender.Before:
                        column.For(a => Html.ActionLink("Подробнее", "Details", new { id = a.Id }/*, new { @class = "PoupupLink", @title = string.Format("{0} {1} {2}", string.IsNullOrEmpty(a.LastName) ? "Ф." : a.LastName, string.IsNullOrEmpty(a.FirstName) ? "И." : a.FirstName, string.IsNullOrEmpty(a.MiddleName) ? "О." : a.MiddleName) }*/)).Encode(false);
                        column.For(a => Html.ActionLink("От имени этого клиента", "SetCurrent", new { id = a.Id })).Encode(false);


                        break;
                    case ColumnRender.After:
                        column.For(model =>
                            RenderAccount(model)                            
                            ).SetName("Account").Named("Счет").Sortable(true).Encode(false);                        
                        column.For(a => Ajax.ActionLink("Удалить", "DeleteClient", new { options = new JavaScriptSerializer().Serialize(new { Filter = Model.FilterContainer, Sort = Model.GridSortOptions }), page = Model.PagedList.PageNumber, id = a.Id }, new AjaxOptions() { Confirm = string.Format("Удалить клиента {0}?", a.OrganizationName), InsertionMode = InsertionMode.Replace, UpdateTargetId = "found_clients" })).Encode(false);
                        break;
                }
            }
        , Html)
    )
    .Sort(Model.GridSortOptions)
    .Attributes(@class => "table-list")
    .Empty("Нет клиентов соответствующих заданному фильтру")
    .RowAlternateColor()
%>