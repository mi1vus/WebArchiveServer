<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebArchiveServer.Pages.Login"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="~/Content/Admin.css" />
</head>
<body>
    <form id="form1" runat="server">
        <h1>Terminal archive: страница входа.</h1>
        <div class="adminContent">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="SingleParagraph" CssClass="error" />

            <div class="loginContainer">
                <div>
                    <label for="name">Имя:</label>
                    <input name="name" />
                </div>
                <div>
                    <label for="password">Пароль:</label>
                    <input type="password" name="password" />
                </div>
                <button type="submit">Войти</button>
            </div>
        </div>
    </form>
</body>
</html>