using System;
using System.Web.Security;

namespace WebArchiveServer.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack)
            {
                string name = Request.Form["name"];
                string password = Request.Form["password"];
                if (name != null && password != null
                        && FormsAuthentication.Authenticate(name, password))
                {
                    FormsAuthentication.SetAuthCookie(name, false);
                    var url = Request["ReturnUrl"];
                    Response.Redirect(url ?? "~/Pages/Index.aspx");
                }
                else
                {
                    ModelState.AddModelError("fail", "Логин или пароль не правильны." +
                        "Пожалуйста введите данные заново");
                }
            }
            else
                FormsAuthentication.SignOut();
        }
    }
}