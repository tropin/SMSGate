using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcContrib.Sorting;

namespace MvcFlan.PagingSorting
{
    /// <summary>
    /// Container for holding Paging/Sorting information
    /// </summary>
    public class PageSortOptions
    {
        public PageSortOptions()
        {
            this.PageSize = 10;
            this.Page = 1;
        }

        public int Page { get; set; }
        public string Column { get; set; }
        public SortDirection Direction { get; set; }
        public int PageSize { get; private set; }
    }
}