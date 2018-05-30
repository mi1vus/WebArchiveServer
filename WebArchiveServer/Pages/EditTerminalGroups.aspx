<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditTerminalGroups.aspx.cs" Inherits="WebArchiveServer.Pages.EditTerminalGroups"  MasterPageFile="~/Pages/Common.Master"%>
<%@ Import Namespace="WebArchiveServer.Helpers"  %>
<%@ Import Namespace="WebArchiveServer.Pages"  %>

<asp:Content ContentPlaceHolderID="head" runat="server">
    Сведенья о группах терминалов
</asp:Content>

<asp:Content ContentPlaceHolderID="user" runat="server">
    <% 
        Response.Write(Common.RegisterUserAndGetProfile(User));
    %>
</asp:Content>

<asp:Content ContentPlaceHolderID="content" runat="server">
    <div id="content">
        <div id="checkoutForm" class="checkout" runat="server">
            <table>
                <%
                    terminals = GetAllTerminals(true);
                    groups = GetAllGroups(true);
                    if (terminals != null && terminals.Any() && groups != null && groups.Any())
                    {
                        int row = 0;
                        foreach (var terminal in terminals)
                        {
                            if (row == 0)
                            {
                                string t_grp_head = string.Empty;
                                foreach (var g in groups) 
                                    t_grp_head += "<th>" + g.Name + "</th>";
                                Response.Write(
                $@"<tr><th>Терм.\Груп.</th>{t_grp_head}</tr>");
                            }
                            string t_grp_row = string.Empty;
                            foreach (var g in groups) 
                                t_grp_row += "<td><input type=\"checkbox\" id=\"chk_" + terminal.Id.ToString() + "_" + g.Id.ToString() + "\" name=\"chk_" + terminal.Id.ToString() + "_" + g.Id.ToString() + "\" value=\"" + (terminal.Groups.ContainsKey(g.Id)? "true": "false") + "\" " + (terminal.Groups.ContainsKey(g.Id)? "checked": "") + "/>" + "</td>";

                            Response.Write(
                $@"<tr><th>{terminal.Name}</th>{t_grp_row}</tr>");
                            ++row;
                        }
                    };
                %>
            </table>
            <p class="actionButtons">
                <button class="actionButtons" type="submit" name="submitButton" value="Save">Сохранить</button>
            </p>
        </div>
        <div id="checkoutMessageOK" runat="server">
            Группировка терминалов была изменена!   
        </div>
        <div id="checkoutMessageNO" runat="server">
            Группировка терминалов не была изменена! Проверьте верность введенных данных или обратитесь к администратору.  
        </div>
    </div>
</asp:Content>














