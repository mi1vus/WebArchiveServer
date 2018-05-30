using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using WebArchiveServer.Helpers;

namespace WebArchiveServer.Pages
{
    public partial class Common : System.Web.UI.MasterPage
    {
        public static int PageSize = 2;
        public static string UserName;

        public static string RegisterUserAndGetProfile(System.Security.Principal.IPrincipal User)
        {
            bool admin = DbHelper.UserIsAdmin(User.Identity.Name);
            var group = DbHelper.UserTerminalGroup(User.Identity.Name);
            UserName = User.Identity.Name;
            return User.Identity.IsAuthenticated ? $"Пользователь: {User.Identity.Name} Admin:{admin} Группа: {group}" : $"Анонимный пользователь!";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack && Request.Form["submitbutton"] != null && Request.Form["submitbutton"] == "SignOut")
            {
                FormsAuthentication.SignOut();
                var url = Request["ReturnUrl"];
                Response.Redirect(url ?? "~/Pages/Index.aspx");
            }
        }
    }
}