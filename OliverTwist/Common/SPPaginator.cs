using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcContrib.Pagination;

namespace Csharper.Common
{
    public class SPPaginator : IPagination<IPagedResult>
    {
        #region IPagination Members

        private IEnumerable<IPagedResult> _paginator;
        private int _pageNumber;
        private int _itemsAtPage;


        public SPPaginator(IEnumerable<IPagedResult> pagination, int pageNumber, int itemsAtPage)
        {
            _paginator = pagination;
            _pageNumber = pageNumber;
            _itemsAtPage = itemsAtPage;
        }
        
        
        public int FirstItem
        {
            get 
            {
                return (int)(_paginator.Min(item => item.Row) ?? 0);
            }
        }

        public bool HasNextPage
        {
            get 
            {
                bool result = false;
                IPagedResult item = _paginator.FirstOrDefault();
                if (item != null)
                {
                    decimal dv = (decimal)(item.Records??0)/(decimal)_itemsAtPage;
                    result = Math.Ceiling(dv) - _pageNumber > 0;
                }
                return result;
            }
        }

        public bool HasPreviousPage
        {
            get { return _pageNumber > 1; }
        }

        public int LastItem
        {
            get { return (int)(_paginator.Max(item => item.Row) ?? 0); }
        }

        public int PageNumber
        {
            get { return _pageNumber; }
        }

        public int PageSize
        {
            get { return _itemsAtPage; }
        }

        public int TotalItems
        {
            get 
            { 
                int result = 0;
                IPagedResult item = _paginator.FirstOrDefault();
                if (item != null)
                {
                    result = item.Records ?? 0;
                }
                return result;
            }
        }

        public int TotalPages
        {
            get 
            {
                int result = 0;
                IPagedResult item = _paginator.FirstOrDefault();
                if (item != null)
                {
                    decimal dv = (decimal)(item.Records ?? 0) / (decimal)_itemsAtPage;
                    result = (int)Math.Ceiling(dv);
                }
                return result;
            }
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((IEnumerable<IPagedResult>)this).GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<IPagedResult> IEnumerable<IPagedResult>.GetEnumerator()
        {
            return _paginator.GetEnumerator();
        }

        #endregion
    }
}