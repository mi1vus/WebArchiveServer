<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebArchiveServer.Pages.Index" %>
<%@ Import Namespace="WebArchiveServer.Models" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Terminals</title>
</head>
<body>
    <form id="form1" runat="server">
         <div class="loginContainer">
            <div>
                <h2>All Terminals</h2>
                <ul id="products" />
                <% 
                    var u = User;
                    Response.Write(u.Identity.IsAuthenticated ? $"Пользователь: {u.Identity.Name}" : $"Анонимный пользователь!");
                %>
            </div>
            <button type="submit">Выйти</button>
       </div>
       <div>
           <table>
                <%
                    int id;
                    if (!int.TryParse(Request.QueryString["idT"], out id))
                    {
                        var terminals = GetAllTerminals();
                        if (terminals.Any())
                        {
                            Response.Write(
                                "<tr><th>Название терминала</th><th>Адрес</th><th>Hasp id</th><th>Заказы</th></tr>");
                            foreach (Terminal terminal in terminals)
                            {
                                Response.Write(String.Format(@"
                            <tr><td>{0}</td><td>{1}</td><td>{2:c}</td><td><a href='/Pages/Index.aspx?idT={3}'>Детально</a></td></tr>",
                                    terminal.Name, terminal.Address, terminal.IdHasp, terminal.Id));
                            }
                        }
                        else
                            Response.Write(
                                "<tr><th>Терминалы не найдены!</th></tr>");
                    }
                    else
                    {
                        WebArchiveServer.Models.Terminal terminal = GetTerminalOrders(id);
                        if (terminal != null)
                        {
                            Response.Write(String.Format(@"
                                <div class='item'>
                                    <h3>Терминал:{0}</h3>
                                    Адрес:{1}<br>
                                    <h4>HaspId:{2:c}</h4>
                                </div>",
                                terminal.Name, terminal.Address, terminal.IdHasp));
                            Response.Write(
                                "<tr><th>Id заказа</th><th>Статус заказа</th><th>сумма</th></tr>");
                            foreach (Order order in terminal.Orders.Values)
                            {
                                Response.Write(String.Format(@"
                            <tr><td>{0}</td><td>{1}</td><td>{2:c}</td></tr>",
                                    order.Id, order.StateName, order.Summ));
                            }
                        }
                        else
                            Response.Write(
                                "<tr><th>Терминал не найден!</th></tr>");
                    }

                %>
           </table>
       </div>
    </form>
    <div>
        <%
            int id;
            if (!int.TryParse(Request.QueryString["idT"], out id))
            {
                for (int i = 1; i <= MaxPageTerminal; i++)
                {
                    Response.Write(
                        String.Format("<a href='/Pages/Index.aspx?pageT={0}' {1}>{2} </a>",
                            i, i == CurrentPageTerminal ? "class='selected'" : "", i));
                }
            }
            else
                for (int i = 1; i <= MaxPageOrder; i++)
                {
                    Response.Write(
                        String.Format("<a href='/Pages/Index.aspx?pageO={0}&idT={3}' {1}>{2}</a>",
                            i, i == CurrentPageOrder ? "class='selected'" : "", i, id));
                }
           Response.Write("<a href='/Pages/Index.aspx'> Список терминалов </a>");
        %>
    </div>
</body>
</html>