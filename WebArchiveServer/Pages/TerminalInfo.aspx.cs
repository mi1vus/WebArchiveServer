using System;
using WebArchiveServer.Helpers;
using WebArchiveServer.Models;

namespace WebArchiveServer.Pages
{
    public partial class TerminalInfo : System.Web.UI.Page
    {
        protected int CurrentPageOrder
        {
            get
            {
                int page;
                page = int.TryParse(Request.QueryString["pageO"], out page) ? page : 1;
                return page > MaxPageOrder ? MaxPageOrder : page <= 0 ? 1 : page;
            }
        }

        protected int MaxPageOrder
        {
            get
            {
                int id;
                if (int.TryParse(Request.QueryString["idT"], out id))
                {
                    decimal count = DbHelper.OrdersCount(Common.UserName, id);
                    if (count <= 0)
                        return 1;

                    return (int)Math.Ceiling(count / Common.PageSize);
                }
                else
                    return 1;
            }
        }

        protected Terminal GetTerminalOrders(int idTerminal)
        {
            if (DbHelper.UpdateTerminalOrders(Common.UserName, idTerminal, CurrentPageOrder, Common.PageSize))
                return DbHelper.Terminals.ContainsKey(idTerminal) ? DbHelper.Terminals[idTerminal] : null;
            else
                return null;
        }

        protected Terminal GetTerminalParameters(int idTerminalP)
        {
            var u = User;
            DbHelper.UpdateTerminalParameters(Common.UserName, idTerminalP);
            //var terminal = terminals.FirstOrDefault(p => p.Key == idTerminalP).Value;
            return DbHelper.Terminals.ContainsKey(idTerminalP) ? DbHelper.Terminals[idTerminalP] : null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}