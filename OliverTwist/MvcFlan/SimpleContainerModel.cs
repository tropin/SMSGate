using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MvcContrib.UI.Grid;

namespace MvcFlan
{
    public class SimpleContainerModel<TModel, TModelFilterContainer>
    {
        /// <summary>
        /// Модель
        /// </summary>
        public TModel Model { get; set; }
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
