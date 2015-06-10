<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcContrib.Pagination.IPagination>" %>
<%@ Import Namespace="MvcContrib.UI.Pager" %>
<p/>
  <%= Html.Pager(Model)
        .Format("Отображается c {0} по {1} из {2} ")
        .SingleFormat("Отображается {0} из {1} ")
        .First("Первая")
        .Last("Последняя")
        .Next("Следующая")
        .Previous("Предыдущая")
         %>
<p/>
