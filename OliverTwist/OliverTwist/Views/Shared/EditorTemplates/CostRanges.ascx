<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Csharper.OliverTwist.Model.CostRangeModel>>" %>
<%@ Import Namespace="MvcContrib" %>
<%@ Import Namespace="MvcContrib.UI.Grid" %>
<%@ Import Namespace="MvcContrib.UI.Grid.ActionSyntax" %>
<%@ Import Namespace="MvcFlan" %>

<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $(document).ready(function () {
        $submitHidden = $('#costRanges > input[type="hidden"]');
        $('form[action]').submit
        (
            function () {
                $newVals = [];
                $("#costRanges tbody > tr[RowId]").each
                (
                    function () {
                        $newVals.push({
                            "Id": $(this).attr('RowId'),
                            "Cost": $(this).find('input[name$="Cost"]')[0].value,
                            "Volume": $(this).find('input[name$="Volume"]')[0].value
                        })
                 });
                $submitHidden[0].value = JSON.stringify($newVals);
            })
        });
    });
    
    function AddCostRange() {    
        $toCopy = $("#costRanges tbody > tr[RowId]").last().clone(true)
        if ($toCopy.length>0)
        {
            $toCopy.attr("RowId", "");
            $toCopy.insertAfter($("#costRanges tbody > tr[RowId]").last());
        }
    }

    function DeleteCostRange(rowObj) {
        if ($("#costRanges tbody > tr[RowId]").length > 1)
        {
            $(rowObj).parentsUntil('tr').parent().remove();  
        }
        return false;
    }

</script>

<div id="costRanges" style="width: 200px; margin-bottom: 20px">
<%: Html.HiddenFor(model => model) %>
<% Html.Grid(Model).Columns(
        column =>
        {
            column.For(x => Html.TextBox("Volume", x.Volume)).Named("Количество");
            column.For(x => Html.TextBox("Cost", x.Cost)).Named("Цена");
            column.For(x => x.Id).Named("Действие").Action(col =>
                {%>
                 <td><a href="#" onclick = "DeleteCostRange(this)">Удалить</a></td>
                 <%});
        }
    )
    .Attributes(@class => "table-list")
    .RowAttributes(data => new Hash(@RowId => data.Item.Id))
    .Empty("Нет данных по ценообразованию")
    .Render();
%>
<span><a href="#" name="addCostRange" onclick="AddCostRange()">Добавить цену</a></span>
</div>