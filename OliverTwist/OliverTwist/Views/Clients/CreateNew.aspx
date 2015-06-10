<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Csharper.OliverTwist.Model.NewClientModel>" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Новый клиент
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Создание нового клиента</h2>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "Клиент не был создан. Поправьте ошибки и попробуйте снова.") %>
        <div>
            <fieldset>
                <legend>Данные клиента</legend>
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.OrganizationName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.OrganizationName)%>
                    <%: Html.ValidationMessageFor(m => m.OrganizationName)%>
                </div>
                
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
                    <input type="submit" value="Создать" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
