using System;
using System.Collections.Generic;
using System.Linq;

internal static class Globals
{
    internal const string OrderByAttributeKey = "OrderByAttribute";
    internal const string SearchFilterAttributeKey = "SearchFilterAttribute";
    internal const string TypeConverterAttributeKey = "TypeConverterAttribute";
    
}

public enum SortOrder
{
    Descending,
    Ascending
}

public enum FilterType
{
    Equals,
    Contains
}