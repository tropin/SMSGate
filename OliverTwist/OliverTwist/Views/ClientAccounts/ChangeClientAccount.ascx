<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Csharper.OliverTwist.Model.ChangeClientAccountModel>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<script runat="server">
    
    private string _controlId = string.Empty;

    public string ControlId
    {
        get
        {
            if (string.IsNullOrEmpty(_controlId))
                _controlId = string.Format("{0}_{1}", "changeClientAccount", Guid.NewGuid());
            return _controlId;
        }
    }

    public bool IsSucessAccountUpdate
    {
        get
        {
            return ViewData["AccountUpdated"] != null && ViewData["AccountUpdated"] is bool && (bool)ViewData["AccountUpdated"];
        }
    }

    public string GetFormName
    {
        get
        {
            return "ChangeClientAccountForm";
        }
    }
    
</script>
<script language="javascript" type="text/javascript">
//alert(costRanges_<%=Model.Id%>[0].Id);
function initCalculator() {
    
    //closure. avoiding garbage in global variables
    (function() {

        var costRanges_<%=Model.Id%> = <%= new JavaScriptSerializer().Serialize(Model.CostRanges) %>;

        //some initial constants
        const formId = "<%=GetFormName%>";
        const OPERATION_FIXED_MONEY = "FixedMoney";
        const OPERATION_FIXED_SMS_AMOUNT = "FixedSMSAmount";
        const OPERATION_FIXED_COST = "FixedCost";

        const EVENT_SOURCE_ADDING_AMOUNT = 10;
        const EVENT_SOURCE_INPUT_MONEY = 20;
        const EVENT_SOURCE_ONE_SMS_COST = 30;

        //initialization
        var currentOperation;
        var smsCostChanged = false; //if false - use cost range data

        function ThrowUnknownOperationException() {
            //TODO: some king of exception should be thrown here... ugh...
        }

        function OneSmsCostChanged() {
            smsCostChanged = true;
            if ($("form#"+formId+" input#OneSMSCost").val() == "") {
                smsCostChanged = false;
            }
        }

        function ValidateFields(eventSource) {
            switch (eventSource) {
                case EVENT_SOURCE_ADDING_AMOUNT:
                    switch (currentOperation) {
                        case OPERATION_FIXED_MONEY:
                            return !(isNaN(parseFloat($("form#"+formId+" input#AddingAmount").val())));
                            break;
                        case OPERATION_FIXED_SMS_AMOUNT:
                            return !(isNaN(parseFloat($("form#"+formId+" input#OneSMSCost").val()))) || !smsCostChanged;
                            break;
                        case OPERATION_FIXED_COST:
                            return !(isNaN(parseFloat($("form#"+formId+" input#AddingAmount").val())));
                            break;
                        default:
                            ThrowUnknownOperationException();
                            break;
                    }
                    break;
                case EVENT_SOURCE_INPUT_MONEY:
                    switch (currentOperation) {
                        case OPERATION_FIXED_MONEY:
                            return !(isNaN(parseFloat($("form#"+formId+" input#OneSMSCost").val()))) || !smsCostChanged;
                            break;
                        case OPERATION_FIXED_SMS_AMOUNT:
                            return !(isNaN(parseFloat($("form#"+formId+" input#InputMoney").val())));
                            break;
                        case OPERATION_FIXED_COST:
                            return !(isNaN(parseFloat($("form#"+formId+" input#InputMoney").val())));
                            break;
                        default:
                            ThrowUnknownOperationException();
                            break;
                    }
                    break;
                case EVENT_SOURCE_ONE_SMS_COST:
                    switch (currentOperation) {
                        case OPERATION_FIXED_MONEY:
                            return !(isNaN(parseFloat($("form#"+formId+" input#OneSMSCost").val()))) || !smsCostChanged;
                            break;
                        case OPERATION_FIXED_SMS_AMOUNT:
                            return !(isNaN(parseFloat($("form#"+formId+" input#OneSMSCost").val()))) || !smsCostChanged;
                            break;
                        case OPERATION_FIXED_COST:
                            return !(isNaN(parseFloat($("form#"+formId+" input#InputMoney").val())));
                            break;
                        default:
                            ThrowUnknownOperationException();
                            break;
                    }
                    break;
                default:
                    ThrowUnknownOperationException();
                    break;
            }
        }

        function GetSmsCost() {
            var currentCostRange = costRanges_<%=Model.Id%>[0];
            for (var costRange in costRanges_<%=Model.Id%>) {
                if (costRange.Volume<currentCostRange.Volume) {
                    currentCostRange = costRange;
                }
            }
            if (!(isNaN(parseFloat($("form#"+formId+" input#InputMoney").val())))) {
                for (var costRange in costRanges_<%=Model.Id%>) {
                    if ((currentCostRange.Volume < costRange.Volume) && (parseFloat(($("form#"+formId+" input#AddingAmount").val())) > costRange.Volume)) {
                        currentCostRange = costRange;
                    }
                }
            }
            if (!(isNaN(parseFloat($("form#"+formId+" input#AddingAmount").val())))) {
                for (var costRange in costRanges_<%=Model.Id%>) {
                    if ((currentCostRange.LowerSum < costRange.LowerSum) && (parseFloat(($("form#"+formId+" input#AddingAmount").val())) > costRange.LowerSum)) {
                        currentCostRange = costRange;
                    }
                }
            }
            return parseFloat(currentCostRange.Cost);
        }

        function RecalcData(eventSource) {
            switch (eventSource) {
                case EVENT_SOURCE_ADDING_AMOUNT:
                    switch (currentOperation) {
                        case OPERATION_FIXED_MONEY:
                            if (ValidateFields(EVENT_SOURCE_ADDING_AMOUNT)) {
                                $("form#"+formId+" input#OneSMSCost").val(parseFloat($("form#"+formId+" input#InputMoney").val())/parseFloat($("form#"+formId+" input#AddingAmount").val()));
                                OneSmsCostChanged();
                            } else {
                            }
                            break;
                        case OPERATION_FIXED_SMS_AMOUNT:
                            if (ValidateFields(EVENT_SOURCE_ADDING_AMOUNT)) {
                                $("form#"+formId+" input#InputMoney").val(parseFloat($("form#"+formId+" input#AddingAmount").val())*(smsCostChanged ? parseFloat($("form#"+formId+" input#OneSMSCost").val()) : GetSmsCost()));
                            } else {
                            }
                            break;
                        case OPERATION_FIXED_COST:
                            if (ValidateFields(EVENT_SOURCE_ADDING_AMOUNT)) {
                                $("form#"+formId+" input#InputMoney").val(parseFloat($("form#"+formId+" input#AddingAmount").val())*(smsCostChanged ? parseFloat($("form#"+formId+" input#OneSMSCost").val()) : GetSmsCost()));
                            } else {
                            }
                            break;
                        default:
                            ThrowUnknownOperationException();
                            break;
                    }
                    break;
                case EVENT_SOURCE_INPUT_MONEY:
                    switch (currentOperation) {
                        case OPERATION_FIXED_MONEY:
                            if (ValidateFields(EVENT_SOURCE_INPUT_MONEY)) {
                                $("form#"+formId+" input#AddingAmount").val(parseFloat($("form#"+formId+" input#InputMoney").val())/(smsCostChanged ? parseFloat($("form#"+formId+" input#OneSMSCost").val()) : GetSmsCost()));
                            } else {
                            }
                            break;
                        case OPERATION_FIXED_SMS_AMOUNT:
                            if (ValidateFields(EVENT_SOURCE_INPUT_MONEY)) {
                                $("form#"+formId+" input#OneSMSCost").val(parseFloat($("form#"+formId+" input#InputMoney").val())/parseFloat($("form#"+formId+" input#AddingAmount").val()));
                                OneSmsCostChanged();
                            } else {
                            }
                            break;
                        case OPERATION_FIXED_COST:
                            if (ValidateFields(EVENT_SOURCE_INPUT_MONEY)) {
                                $("form#"+formId+" input#AddingAmount").val(parseFloat($("form#"+formId+" input#InputMoney").val())/(smsCostChanged ? parseFloat($("form#"+formId+" input#OneSMSCost").val()) : GetSmsCost()));
                            } else {
                            }
                            break;
                        default:
                            ThrowUnknownOperationException();
                            break;
                    }
                    break;
                case EVENT_SOURCE_ONE_SMS_COST:
                    switch (currentOperation) {
                        case OPERATION_FIXED_MONEY:
                            if (ValidateFields(EVENT_SOURCE_ONE_SMS_COST)) {
                                $("form#"+formId+" input#AddingAmount").val(parseFloat($("form#"+formId+" input#InputMoney").val())/(smsCostChanged ? parseFloat($("form#"+formId+" input#OneSMSCost").val()) : GetSmsCost()));
                            } else {
                            }
                            break;
                        case OPERATION_FIXED_SMS_AMOUNT:
                            if (ValidateFields(EVENT_SOURCE_ONE_SMS_COST)) {
                                $("form#"+formId+" input#InputMoney").val(parseFloat($("form#"+formId+" input#AddingAmount").val())*(smsCostChanged ? parseFloat($("form#"+formId+" input#OneSMSCost").val()) : GetSmsCost()));
                            } else {
                            }
                            break;
                        case OPERATION_FIXED_COST:
                            if (ValidateFields(EVENT_SOURCE_ONE_SMS_COST)) {
                                $("form#"+formId+" input#InputMoney").val(parseFloat($("form#"+formId+" input#AddingAmount").val())*(smsCostChanged ? parseFloat($("form#"+formId+" input#OneSMSCost").val()) : GetSmsCost()));
                            } else {
                            }
                            break;
                        default:
                            ThrowUnknownOperationException();
                            break;
                    }
                    break;
                default:    
                    ThrowUnknownOperationException();
                    break;
            }
            
        }
        
        function OperationChanged() {

            switch ($("form#"+formId+" input[name='FixedRow']:checked").val()) {
                case OPERATION_FIXED_MONEY:
                    currentOperation = OPERATION_FIXED_MONEY;
                    break;
                case OPERATION_FIXED_SMS_AMOUNT:
                    currentOperation = OPERATION_FIXED_SMS_AMOUNT;
                    break;
                case OPERATION_FIXED_COST:
                    currentOperation = OPERATION_FIXED_COST;
                    break;
                default:
                    ThrowUnknownOperationException();
                    break; 
            }

            $("form#"+formId+" input#CalcMode").val(currentOperation);

            RecalcData();

        }

        //invoke initial
        OperationChanged();

        //bind listeners
        $("form#"+formId+" input[name='FixedRow']").change(function() {
            OperationChanged();
        });
        $("form#"+formId+" input#AddingAmount").keyup(function() {
            RecalcData(EVENT_SOURCE_ADDING_AMOUNT);
        });
        $("form#"+formId+" input#InputMoney").keyup(function() {
            RecalcData(EVENT_SOURCE_INPUT_MONEY);
        });
        $("form#"+formId+" input#OneSMSCost").keyup(function() {
            OneSmsCostChanged();
            RecalcData(EVENT_SOURCE_ONE_SMS_COST);
        });

    })();
}
</script>
<%  using (Ajax.BeginForm("ChangeClientAccount", null, new AjaxOptions() { OnSuccess = "SuccessClosePopup", UpdateTargetId = ControlId, HttpMethod = "POST" }, new { id = GetFormName }))
    {%>
<div id="<%=ControlId%>">
    <%: Html.HiddenFor(model => model.Id)%>
    <%: Html.Hidden("CalcMode", Model.CalcMode)%>
    <div>
        <div class="account_change_datarow">
            <div class="account_change_label">
                Текущий баланс
            </div>
            <div class="account_change_textbox">
                <%: Model.Account %>
            </div>
        </div>
        <div class="account_change_datarow">
            <div class="account_change_label">
                Количество
            </div>
            <div class="account_change_checkbox">
                <input type="radio" name="FixedRow" value="FixedSMSAmount" />
            </div>
            <div class="account_change_textbox">
                <%: Html.TextBox("AddingAmount", Model.AddingAmount) %>
            </div>
        </div>
        <div class="account_change_datarow">
            <div class="account_change_label">
                Цена
            </div>
            <div class="account_change_checkbox">
                <input type="radio" name="FixedRow" value="FixedCost" checked="checked" />
            </div>
            <div class="account_change_textbox">
                <%: Html.TextBox("OneSMSCost", Model.OneSMSCost) %>
            </div>
        </div>
        <div class="account_change_datarow">
            <div class="account_change_label">
                Сумма
            </div>
            <div class="account_change_checkbox">
                <input type="radio" name="FixedRow" value="FixedMoney" />
            </div>
            <div class="account_change_textbox">
                <%: Html.TextBox("InputMoney", Model.InputMoney) %>
            </div>
        </div>
        <div class="account_change_datarow">
            <div class="float_right clear_right">
                <input type="submit" value="Внести" class="btnNeutral" />
            </div>
        </div>
    </div>
    <%if (IsSucessAccountUpdate)
      { %>
    <div id="sucсess">
        Успешно внесено!</div>
    <%} %>
</div>
<% }%>
<p />
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        initCalculator();
    });
</script>
