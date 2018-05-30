using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebArchiveServer.Helpers;
using WebArchiveServer.Models;

namespace WebArchiveServer.Pages
{
    public partial class AddTerminal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            checkoutForm.Visible = true;
            checkoutMessageOK.Visible = false;
            checkoutMessageNO.Visible = false;

            if (IsPostBack && Request.Form["submitbutton"] != null && Request.Form["submitbutton"] == "Add")
            {
                Terminal terminal = new Terminal();
                if (TryUpdateModel(terminal,
                   new FormValueProvider(ModelBindingExecutionContext)))
                {
                    var a = terminal.Address;
                    checkoutForm.Visible = false;
                    if (DbHelper.AddTerminal(terminal.IdHasp, terminal.Name, terminal.Address, Common.UserName))
                    {
                        checkoutMessageOK.Visible = true;
                        checkoutMessageNO.Visible = false;
                    }
                    else
                    {
                        checkoutMessageOK.Visible = false;
                        checkoutMessageNO.Visible = true;
                    }
                    checkoutForm.Visible = false;
                }
            }
        }
    }
}