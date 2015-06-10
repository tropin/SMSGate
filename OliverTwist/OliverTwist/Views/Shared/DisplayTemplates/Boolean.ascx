<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
	private bool? ModelValue
	{
		get
		{
			bool? value = null;
			if (ViewData.Model != null)
			{
				value = Convert.ToBoolean(ViewData.Model, System.Globalization.CultureInfo.InvariantCulture);
			}
			return value;
		}
	}

    private string _pathFormat = "../../../Content/Images/{0}.png";
    
    private string ImagePath
    {
        get
        {
            string result = string.Empty;
            if (!ModelValue.HasValue)
                result = ResolveUrl(string.Format(_pathFormat, "null"));
            else if (ModelValue.Value)
            {
                result = ResolveUrl(string.Format(_pathFormat, "yes"));
            }
            else
            {
                result = ResolveUrl(string.Format(_pathFormat, "no"));
            } 
            return result;
        }
    }
</script>
<image width="32" height="32" src="<%= ImagePath %>"/>