<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddTerminal.aspx.cs" Inherits="WebArchiveServer.Pages.AddTerminal" MasterPageFile="~/Pages/Common.Master"%>
<%@ Import Namespace="WebArchiveServer.Helpers"  %>
<%@ Import Namespace="WebArchiveServer.Pages"  %>

<asp:Content ContentPlaceHolderID="head" runat="server">
    Сведенья о терминалах
</asp:Content>

<asp:Content ContentPlaceHolderID="user" runat="server">
    <% 
        Response.Write(Common.RegisterUserAndGetProfile(User));
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="content" runat="server">
    <div id="content">
        <div>
            <div id="checkoutForm" class="checkout" runat="server">
                <h2>Добавление терминала</h2>
                Пожалуйста, введите данные терминала!

                <div id="errors" data-valmsg-summary="true">
                    <ul>
                        <li style="display:none"></li>
                    </ul>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                </div>

                <h3>Терминал</h3>
                <div>
                    <label for="Name">Название:</label>
                    <SX:VInput Property="Name" runat="server" />
                </div>
                <div>
                    <label for="Address">Адрес терминала:</label>
                    <SX:VInput Property="Address" runat="server" />
                </div>
                <div>
                    <label for="IdHasp">Hasp id:</label>
                    <SX:VInput Property="IdHasp" runat="server" />
                </div>
       
                <p class="actionButtons">
                    <button class="actionButtons" type="submit" name="submitButton" value="Add">Добавить терминал</button>
                </p>
            </div>
            <div id="checkoutMessageOK" runat="server">
                Терминал был добавлен!   
            </div>
            <div id="checkoutMessageNO" runat="server">
                Терминал не был добавлен! Проверьте верность введенных данных или обратитесь к администратору.  
            </div>
        </div>
    </div>
</asp:Content>
