<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebArchiveServer.Pages.Index"  MasterPageFile="~/Pages/Common.Master"%>
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
                    //int id;
                    //if (!int.TryParse(Request.QueryString["idT"], out id))
                    //{
                    var terminals = GetTerminals();
                    if (terminals != null && terminals.Any())
                    {
                        Response.Write(
                            "<tr><th>Название терминала</th><th>Номер группы</th><th>Группа</th><th>Адрес</th><th>Hasp id</th><th>Заказы</th></tr>");
                        foreach (var terminal in terminals)
                        {
                            string t_grp_ids = terminal.Groups.Values.Any() ? 
                                terminal.Groups.Values.Select(t => t.Id.ToString()).Aggregate((current, next) => current + ", " + next) : 
                                " - ";
                            string t_grp_nms = terminal.Groups.Values.Any() ? 
                                terminal.Groups.Values.Select(t => t.Name).Aggregate((current, next) => current + ", " + next) : 
                                " - ";
                            Response.Write(String.Format(@"
                            <tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td><a href='/Pages/TerminalInfo.aspx?idT={5}'>Детально</a></td></tr>",
                                terminal.Name, t_grp_ids, t_grp_nms, terminal.Address, terminal.IdHasp, terminal.Id));
                        }
                    }
                    else
                        Response.Write(
                            "<tr><th>Терминалы не найдены!</th></tr>");
                    //}
                    //else
                    //{
                    //        Response.Write(
                    //            "<tr><th>Терминал не найден!</th></tr>");
                    //}

                %>
            </table>
        </div>
        <div class="pager">
            <%
                //int id;
                //if (!int.TryParse(Request.QueryString["idT"], out id))
                //{
                for (var i = 1; i <= MaxPageTerminal; i++)
                {
                    Response.Write(
                        String.Format("<a href='/Pages/Index.aspx?pageT={0}' {1}>{2} </a>",
                            i, i == CurrentPageTerminal ? "class='selected'" : "", i));
                }
                //}
                //else
                //    for (var i = 1; i <= MaxPageOrder; i++)
                //    {
                //        Response.Write(
                //            String.Format("<a href='/Pages/Index.aspx?pageO={0}&idT={3}' {1}>{2}</a>",
                //                i, i == CurrentPageOrder ? "class='selected'" : "", i, id));
                //    }
            %>
        </div>
    </div>
</asp:Content>
