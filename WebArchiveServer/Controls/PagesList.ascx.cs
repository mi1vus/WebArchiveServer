using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using WebArchiveServer.Models;

namespace WebArchiveServer.Controls
{
    public partial class PagesList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected string CreateHomeLinkHtml()
        {
            string path = "'/Pages/Index.aspx'"; // RouteTable.Routes.GetVirtualPath(null, null).VirtualPath;
            return string.Format("<a href='{0}'>Главная</a>", path);
        }

        protected IEnumerable<string> GetPages()
        {
            return new List<string> {
                "<a href='/Pages/Index.aspx'> Список терминалов </a>",
                "<a href='/Pages/AddTerminal.aspx'> Новый терминал </a>",
                "<a href='/Pages/EditTerminalGroups.aspx'> Редактирование групп терминалов </a>"
            };
        }
    }
}