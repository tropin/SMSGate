<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Csharper.OliverTwist.Model.RegisterModel>" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Регистрация в системе
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Созание учетной записи</h2>
    <p>
        Заполните поля формы для регистрации. 
    </p>
    <p>
        Пароль должен быть как минимум <%: ViewData["PasswordLength"] %> символов.
    </p>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "Регистрация не была осуществлена. Поправьте ошибки и попробуйте снова.") %>
        <div>
            <fieldset>
                <legend>Ваши данные</legend>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.UserName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName) %>
                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Email) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.Email) %>
                    <%: Html.ValidationMessageFor(m => m.Email) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Password) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </div>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                <hr />

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.FirstName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.FirstName)%>
                    <%: Html.ValidationMessageFor(m => m.FirstName)%>
                </div>

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.MiddleName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.MiddleName)%>
                    <%: Html.ValidationMessageFor(m => m.MiddleName)%>
                </div>

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.LastName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.LastName)%>
                    <%: Html.ValidationMessageFor(m => m.LastName)%>
                </div>

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.City) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.City)%>
                    <%: Html.ValidationMessageFor(m => m.City)%>
                </div>

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.OrganizationName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.OrganizationName)%>
                    <%: Html.ValidationMessageFor(m => m.OrganizationName)%>
                </div>

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.MobilePhone) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.MobilePhone)%>
                    <%: Html.ValidationMessageFor(m => m.MobilePhone)%>
                </div>

                <!--<div class="editor-label">
                    <%: Html.LabelFor(m => m.TimeZone) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.TimeZone)%>
                    <%: Html.ValidationMessageFor(m => m.TimeZone)%>
                </div>-->

                <p> 
                    <input type="submit" value="Зарегистрироваться" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
