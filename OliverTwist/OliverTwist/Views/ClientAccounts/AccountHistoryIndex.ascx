<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>      

<script runat = "server">
    
    private string _controlId = string.Empty;
    
    public string ControlId
    {
        get
        {
            if (string.IsNullOrEmpty(_controlId))
                _controlId = string.Format("{0}_{1}", "accountHistory", Guid.NewGuid());
            return _controlId;
        }
    }

    private System.Dynamic.ExpandoObject SearchParam
    {
        get
        {
            dynamic dynObj = new System.Dynamic.ExpandoObject();
            dynObj.Model = Model.AccountHistory.FilterContainer;
            dynObj.ClientId = Model.ClientId;
            dynObj.ControlIdToRefresh = ControlId;
            return dynObj;
        }
    }
        
</script>
            <% 
                RenderPartialExtensions.RenderPartial(Html, "AccountHistorySearchFilter", SearchParam); 
            %>
            <div id="<%=ControlId%>">
                        <% RenderPartialExtensions.RenderPartial(Html,"AccountHistoryWithPaging", Model.AccountHistory); %>           
            </div>





