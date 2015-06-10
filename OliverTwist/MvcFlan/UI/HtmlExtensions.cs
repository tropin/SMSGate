using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcContrib.UI.Grid;
using MvcContrib.Sorting;
using System.Web.Routing;
using MvcContrib.UI.Grid.Syntax;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace MvcFlan
{
    public static class HtmlExtensions
    {
        public static IGridWithOptions<T> RowAlternateColor<T>(this IGridWithOptions<T> grid) where T : class
        {
            grid.Model.Sections.RowStart(a => (a.IsAlternate) ? "<tr class='tr-alt-item'>" : "<tr>");
            return grid;
        }

        public static IGridColumn<T> SetName<T>(this IGridColumn<T> column, string name)
        {
            column.GetType().GetField("_name", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(column, name);
            return column;
        }

        public static MvcHtmlString GetLinkLabel(this HtmlHelper helper, string attributeName, string actionName, string innerText, string controllerName, object routeValues)
        {
            return GetLinkLabel(helper, attributeName, actionName, innerText, controllerName, routeValues, null);
        }
        
        public static MvcHtmlString GetLinkLabel(this HtmlHelper helper, string attributeName, string actionName, string innerText, string controllerName, object routeValues, object attributes)
        {
            TagBuilder tb = new TagBuilder("span");
            RouteValueDictionary attrs = new RouteValueDictionary(attributes);
            UrlHelper urlHepler = new UrlHelper(helper.ViewContext.RequestContext);
            string resultingLink = string.Empty;
            if (string.IsNullOrEmpty(controllerName))
                resultingLink = urlHepler.Action(actionName, routeValues);
            else
                resultingLink = urlHepler.Action(actionName, controllerName, routeValues);
            tb.Attributes.Add(attributeName, resultingLink);
            foreach (var attr in attrs)
            {
                tb.Attributes.Add(attr.Key, attr.Value.ToString());
            }
            tb.SetInnerText(innerText);
            return MvcHtmlString.Create(tb.ToString(TagRenderMode.Normal));
        }


        public static string GetDisplayName(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DisplayAttribute attrs = (DisplayAttribute)field.
                     GetCustomAttributes(typeof(DisplayAttribute), false).First();
            return attrs.GetName();
        }
        
        public static SelectList ToSelectList<T>(this T enumeration, bool NotSelect = false, Type enumType = null)
        {
            var items = new Dictionary<object, string>();
            Array source = null;
            if (enumType != null)
                source = Enum.GetValues(enumType);
            else if (enumeration != null)
                source = Enum.GetValues(enumeration.GetType());
            else if (enumeration == null && typeof(T) != typeof(Enum))
                source = Enum.GetValues(typeof(T));
            else source = new object[0];
            var displayAttributeType = typeof(DisplayAttribute);
            foreach (var value in source)
            {
                FieldInfo field = value.GetType().GetField(value.ToString());
                DisplayAttribute attrs = (DisplayAttribute)field.
                        GetCustomAttributes(displayAttributeType, false).FirstOrDefault();
                items.Add(value, attrs == null?field.Name: attrs.GetName());
            }
            return NotSelect?new SelectList(items, "Key", "Value"): new SelectList(items, "Key", "Value", enumeration);
        }
    }

}