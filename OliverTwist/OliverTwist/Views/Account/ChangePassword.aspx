<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Csharper.OliverTwist.Model.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Смена пароля
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Сменить пароль</h2>
    <p>
        Используйте данную форму для смены пароля
    </p>
    <p>
        Новые пароли должны быть минимум <%: ViewData["PasswordLength"] %> символов в длинну.
    </p>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "Смена пароля неудачна. Пожалуйста исправьте ошибки и повторите операцию.") %>
        <div>
            <fieldset>
                <legend>Информация о профиле</legend>
                
                <% if (Model is Csharper.OliverTwist.Model.ChangePasswordWithOldModel)
                   { %>
                <div class="editor-label">
                    <%: Html.LabelFor(m => (Model as Csharper.OliverTwist.Model.ChangePasswordWithOldModel).OldPassword)%>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => (Model as Csharper.OliverTwist.Model.ChangePasswordWithOldModel).OldPassword)%>
                    <%: Html.ValidationMessageFor(m => (Model as Csharper.OliverTwist.Model.ChangePasswordWithOldModel).OldPassword)%>
                </div>
                <% } %>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.NewPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.NewPassword) %>
                    <%: Html.ValidationMessageFor(m => m.NewPassword) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                
                <p>
                    <input type="submit" value="Сменить пароль" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
