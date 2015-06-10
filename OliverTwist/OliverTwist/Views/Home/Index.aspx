<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    АДЕ СМС - Оперативная доставка важной информации
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    Текущие реализованные функции:
    <ul>
        <li>Регистрация пользователей, вход, выход, изменение профиля, изменение пароля, восстановление пароля</li>
        <li>Список клиентов, он же по совместительству адресная книга.</li>
        <li>Добавление клиентов</li>
        <li>Просмотр деталей по клиентам</li>
    </ul>
</asp:Content>
