using System;
using System.Web.Security;
using WebArchiveServer.Helpers;

namespace WebArchiveServer.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                var name = Request.Form["name"];
                var password = Request.Form["password"];

                if (DbHelper.IsAuthorizeUser(name, password))
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