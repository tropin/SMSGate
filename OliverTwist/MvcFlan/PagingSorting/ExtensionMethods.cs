using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Collections.Generic;
using MvcContrib.Pagination;
using MvcContrib.Sorting;
using System.Reflection;
using MvcFlan.Attributes;

namespace MvcFlan.PagingSorting
{
    public static class ExtensionMethods
    {
        const string NoDefaultOrderByAttributeFound = "When paging or sorting, your model must have at least one field with [OrderBy(IsDefault = true)]";

        /// <summary>
        /// Adds filter criteria to the IQueryable based on the properties of the 
        /// filter of type T.
        /// For a filter to be applied, the field has to be decorated with 
        /// [SearchFilter()] and the value passed in should not be null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<T> AsFiltered<T>(this IQueryable<T> queryable, T filter)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(filter);
            Type filterType = typeof(T);

            foreach (ModelMetadata modelMetaData in GetModelMetadataByAdditionalValueKey(filterType, Globals.SearchFilterAttributeKey))
            {
                PropertyInfo propInfo = typeof(T).GetProperty(modelMetaData.PropertyName);
                object propertyValue = propInfo.GetValue(filter, null);
                string propertyName = modelMetaData.PropertyName;
                var searchFilterAttribute = (SearchFilterAttribute)modelMetaData.AdditionalValues[Globals.SearchFilterAttributeKey];

                Type rightType = modelMetaData.ModelType;

                if (!(propertyValue == null || (propertyValue is string && string.IsNullOrEmpty(propertyValue as string))))
                {
                    var param = Expression.Parameter(filterType, propertyName);
                    Expression left =  Expression.Property(param, filterType.GetProperty(propertyName));
                    
                    var right = Expression.Constant(propertyValue, rightType);
                    
                    LambdaExpression predicate = null;

                    if (searchFilterAttribute.FilterType == FilterType.Contains) //FilterType is Contains
                    {
                        var methodContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var filterContains = Expression.Call(left, methodContains, right);
                        predicate = Expression.Lambda(filterContains, param);
                    }
                    else //FilterType is Equals
                    {
                        var expr = Expression.Equal(left, right);
                        predicate = Expression.Lambda(expr, param);

                    }

                    var expression = Expression.Call(typeof(Queryable), "Where", new Type[] { queryable.ElementType },
                             queryable.Expression,
                             predicate);
                    queryable = queryable.Provider.CreateQuery<T>(expression);
                }
            }

            return queryable;
        }

        /// <summary>
        /// Converts an IQueryable into an IPagination using the PageSortOptions.
        /// Sorting will be done on the sortField passed in the PageSortOptions
        /// if that field has the attribute [OrderBy()]. If not the field that 
        /// has [OrderBy(IsDefault=true)] is used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="paged"></param>
        /// <returns></returns>
        public static IPagination<T> AsPagination<T>(this IQueryable<T> source, PageSortOptions paged)
        {
            var modelMetadata = GetModelMetadataForOrderByField(paged.Column, typeof(T));
            var sortOrder = (paged.Direction == SortDirection.Ascending)? 
                                "OrderBy" : "OrderByDescending";


            ParameterExpression[] parameters = new ParameterExpression[] {
                Expression.Parameter(typeof(T)) };

            MemberExpression memberExpression = Expression.Property(parameters[0], modelMetadata.PropertyName);
            Expression queryExpr = Expression.Call(
                    typeof(Queryable), sortOrder,
                    new Type[] { typeof(T), modelMetadata.ModelType },
                    source.Expression, Expression.Quote(Expression.Lambda(memberExpression, parameters)));
            source = source.Provider.CreateQuery<T>(queryExpr);

            return source.AsPagination(paged.Page, paged.PageSize);
        }


        /// <summary>
        /// Gets the ModelMedataData for the field being sorted on. 
        /// If the field being sorted on does not have the OrderBy attribute,
        /// we return the ModelMetaData for the field that has IsDefault set to true.
        /// [OrderBy(IsDefault = true)]
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="iqueryableType"></param>
        /// <returns></returns>
        private static ModelMetadata GetModelMetadataForOrderByField(string sortField, Type iqueryableType)
        {
            //Ensure field being sorted on has OrderBy attribute
            var modelMetadata = GetModelMetadataByAdditionalValueKey(iqueryableType, Globals.OrderByAttributeKey)
                                .Where(a => a.PropertyName.Equals(sortField, StringComparison.OrdinalIgnoreCase))
                                .SingleOrDefault();

            //If field not found, find default OrderBy attribute
            if (modelMetadata == null)
            {
                modelMetadata = GetModelMetadataByAdditionalValueKey(iqueryableType, Globals.OrderByAttributeKey)
                                .Where(a => ((OrderByAttribute) a.AdditionalValues[Globals.OrderByAttributeKey]).IsDefault)
                                .SingleOrDefault();

                if (modelMetadata == null)
                {
                    throw new InvalidOperationException(NoDefaultOrderByAttributeFound);
                }
            }
            return modelMetadata;
        }


        /// <summary>
        /// Find all ModelMetaData that have AdditionalValues with the given key
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelType"></param>
        /// <param name="attributeKey"></param>
        /// <returns></returns>
        private static IEnumerable<ModelMetadata> GetModelMetadataByAdditionalValueKey(Type modelType, string attributeKey)
        {
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, modelType);
            return modelMetadata.Properties.Where(a => a.AdditionalValues.Keys.Contains(attributeKey));
        }

    }
}
