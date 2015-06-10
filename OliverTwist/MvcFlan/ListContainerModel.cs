using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcContrib.Pagination;
using MvcContrib.UI.Grid;
using System.ComponentModel.DataAnnotations;

namespace MvcFlan
{
    public class ListContainerModel<TModel, TModelFilterContainer>
    {
        /// <summary>
        /// Паджинированный список объетов модели
        /// </summary>
        public IPagination<TModel> PagedList { get; set; }
        /// <summary>
        /// Контейнер данных для фильтров
        /// </summary>
        [DataType("FilterContainer")]
        public TModelFilterContainer FilterContainer { get; set; }
        /// <summary>
        /// Настройки сортировки
        /// </summary>
        public GridSortOptions GridSortOptions { get; set; } 
    }
}
