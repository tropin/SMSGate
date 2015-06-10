using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MvcContrib.UI.Grid;
using MvcContrib.UI.Grid.ActionSyntax;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace MvcFlan
{
    public enum ColumnRender
    {
        Before,
        After
    }
    
    /// <summary>
    /// Class to auto generate columns based on the ModelMetadata
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MetaDataGridModel<T> : GridModel<T> where T : class, new()
    {

        public MetaDataGridModel() : this(null, null){}

        /// <summary>
        /// Enumerates through the ModelMetadata for a given type and
        /// adds columns to the Grid
        /// </summary>
        /// <param name="insertColumns"></param>
        public MetaDataGridModel(Action<ColumnBuilder<T>, ColumnRender> insertColumns, HtmlHelper html)
        {
            //To insert any custom columns the user wishes to add in front of the auto generated ones.
            //Ideally the MVCContrib Grid should allow inserting columns
            insertColumns(this.Column, ColumnRender.Before);

            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(T));
            foreach (var modelMetadataProperty in modelMetadata.Properties)
            {
                if (modelMetadataProperty.ShowForDisplay)
                {
                    bool isSortable = modelMetadataProperty.AdditionalValues.ContainsKey(Globals.OrderByAttributeKey);
                    string displayFormatString = modelMetadataProperty.DisplayFormatString;
                    var expression = CreateExpression(modelMetadataProperty, html);

                    IGridColumn<T> column = Column.For(expression)
                                                .Named(modelMetadataProperty.GetDisplayName())
                                                .Sortable(isSortable)
                                                .SetName(modelMetadataProperty.PropertyName)
                                                .Encode(false);
                    

                    if (!string.IsNullOrEmpty(displayFormatString))
                    {
                        column.Format(displayFormatString);
                    }
                }
            }
            insertColumns(this.Column, ColumnRender.After);
        }


        private static Expression<Func<T, object>> CreateExpression(ModelMetadata modelMetadata, HtmlHelper html)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            Expression expression = Expression.Property(param, modelMetadata.PropertyName);
            if (html != null)
            {
                Type realModelType = html.GetType().GetGenericArguments()[0];
                MethodInfo displayMI = typeof(DisplayExtensions).GetMethods().SingleOrDefault(method => method.Name == "DisplayFor" &&
                   method.IsGenericMethod == true && method.ContainsGenericParameters == true && method.GetParameters().Count() == 2);
                displayMI = displayMI.MakeGenericMethod(realModelType, modelMetadata.ModelType);
                ParameterExpression param2 = Expression.Parameter(realModelType, "y");
                expression = Expression.Call(displayMI, Expression.Constant(html), Expression.Lambda(expression, param2));
            }
            if (modelMetadata.ModelType.IsValueType)
            {
                expression = Expression.Convert(expression, typeof(object));
            }
            var predicate = Expression.Lambda<Func<T, object>>(expression, param);
            return predicate;
        }

        static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly,
        Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType.IsAssignableFrom(extendedType)
                        select method;
            return query;
        }


    }
 
}