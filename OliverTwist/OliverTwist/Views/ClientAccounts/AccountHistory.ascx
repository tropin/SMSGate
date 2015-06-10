<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcFlan.ListContainerModel<Csharper.OliverTwist.Model.ClientAccountActionModel, OliverTwist.FilterContainers.AccountHistoryFilterContainer>>" %>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcFlan" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%= Html.Grid(Model.PagedList)
    .WithModel(
            new MetaDataGridModel<Csharper.OliverTwist.Model.ClientAccountActionModel>((column, renderWhen) =>
            {
                switch (renderWhen)
                {
                    case ColumnRender.After:

                        column.For(a => a.DistributionId.HasValue ?
                            Ajax.ActionLink("Рассылка", "BLABLABLA1", new { distributionId = a.DistributionId }, new AjaxOptions()) :
                            a.TargetAccountId.HasValue? 
                            Ajax.ActionLink(string.Format("Зачислено в пользу {0}",a.TargetAccountOrganizationName) , "BLABLABLA2", 
                            new { targetAccountId = a.TargetAccountId }, new AjaxOptions()):
                            MvcHtmlString.Empty).SetName("Distribution").Named("Статья расходов").Sortable(false).Encode(false);
                        break;
                }
            }
        , Html)
    )
    .Sort(Model.GridSortOptions)
    .Attributes(@class => "table-list")
    .Empty("Нет записей в истории соответствующих заданному фильтру")
    .RowAlternateColor()
%>