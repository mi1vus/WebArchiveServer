<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TerminalInfo.aspx.cs" Inherits="WebArchiveServer.Pages.TerminalInfo" MasterPageFile="~/Pages/Common.Master"%>
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
            <table>
                <%
                    int id;
                    if (!int.TryParse(Request.QueryString["idT"], out id))
                    {
                        Response.Write(
                            "<tr><th>Не указан номер нужного терминала!</th></tr>");
                    }
                    else
                    {
                        var terminal = GetTerminalOrders(id);
                        if (terminal != null)
                        {
                            GetTerminalParameters(id);
                        }
                        if (terminal != null)
                        {
                            string t_grp_ids = terminal.Groups.Values.Any() ? 
                                terminal.Groups.Values.Select(t => t.Id.ToString()).Aggregate((current, next) => current + ", " + next) : 
                                " - ";
                            string t_grp_nms = terminal.Groups.Values.Any() ? 
                                terminal.Groups.Values.Select(t => t.Name).Aggregate((current, next) => current + ", " + next): 
                                " - ";
                            Response.Write(String.Format(@"
                                <div class='item'>
                                    <h3>Терминал:{0} Группа№{1}:{2}</h3>
                                    Адрес:{3}<br>
                                    <h4>HaspId:{4}</h4>
                                </div>",
                                terminal.Name, t_grp_ids, t_grp_nms, terminal.Address, terminal.IdHasp));

                            Response.Write(
                                "<tr><th>Id параметра</th><th>Терминал</th><th>Название</th><th>Путь</th><th>Значение</th><th>Сохранен</th></tr>");
                            foreach (var parametr in terminal.Parameters)
                            {
                                Response.Write(String.Format(@"
                                <tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>",
                                    parametr.Id,parametr.TId, parametr.Name, parametr.Path, parametr.Value, parametr.SaveTime >= parametr.LastEditTime));
                            }

                            Response.Write(
                                "<tr><th>Id заказа</th><th>RNN</th><th>Статус заказа</th><th>Топливо</th><th>Тип оплаты</th><th>Цена до налива</th><th>Окончательная цена</th><th>Количество до налива</th><th>Окончательное количество</th><th>Сумма до налива</th><th>Окончательная сумма</th></tr>");
                            foreach (var order in terminal.Orders.Values)
                            {
                                Response.Write(String.Format(@"
                                <tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5:c}</td><td>{6:c}</td><td>{7:c}</td><td>{8:c}</td><td>{9:c}</td><td>{10:c}</td></tr>",
                                    order.Id, order.Rnn, order.StateName, order.FuelName, order.PaymentName, order.PrePrice, order.Price, order.PreQuantity, order.Quantity, order.PreSumm, order.Summ));
                            }
                        }
                        else
                            Response.Write(
                                "<tr><th>Терминал не найден!</th></tr>");
                    }

                %>
            </table>
        </div>
        <div  class="pager">
            <%
                //if (int.TryParse(Request.QueryString["idT"], out id))
                for (var i = 1; i <= MaxPageOrder; i++)
                {
                    Response.Write(
                        String.Format("<a href='/Pages/TerminalInfo.aspx?pageO={0}&idT={3}' {1}>{2}</a>",
                            i, i == CurrentPageOrder ? "class='selected'" : "", i, id));
                }
            %>
        </div>
    </div>
</asp:Content>
