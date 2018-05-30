using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebArchiveServer.Helpers;
using WebArchiveServer.Models;

namespace WebArchiveServer.Pages
{
    public partial class EditTerminalGroups : System.Web.UI.Page
    {
        protected IEnumerable<Terminal> terminals;
        protected IEnumerable<Group> groups;

        protected void Page_Load(object sender, EventArgs e)
        {
            checkoutForm.Visible = true;
            checkoutMessageOK.Visible = false;
            checkoutMessageNO.Visible = false;


            if (IsPostBack && Request.Form["submitbutton"] != null && Request.Form["submitbutton"] == "Save")
            {
                var result = false;
                terminals = GetAllTerminals(true);
                groups = GetAllGroups(true);
                if (terminals != null && terminals.Any() && groups != null && groups.Any())
                {
                    var new_checked = Request.Form.AllKeys;
                    List<TerminalGroup> to_delete = new List<TerminalGroup>();
                    List<TerminalGroup> to_add = new List<TerminalGroup>();
                    foreach (var terminal in terminals)
                    {
                        foreach (var group in groups)
                        {
                            bool in_new_checked = new_checked.Any(k => k == $"chk_{terminal.Id}_{group.Id}");
                            bool in_old_checked = terminal.Groups.Any(g => g.Key == group.Id);

                            if (in_new_checked && in_old_checked)
                                continue;
                            else if (in_new_checked && !in_old_checked)
                                to_add.Add(new TerminalGroup
                                {
                                    IdTerminal = terminal.Id,
                                    IdGroup = group.Id
                                });
                            else if (!in_new_checked && in_old_checked)
                                to_delete.Add(new TerminalGroup
                                {
                                    IdTerminal = terminal.Id,
                                    IdGroup = group.Id
                                });
                        }
                    }
                    result = DbHelper.UpdateGroups(to_add, to_delete, Common.UserName);
                }
                if (result)
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

    protected IEnumerable<Terminal> GetAllTerminals(bool update)
        {
            if (!update && DbHelper.Terminals != null)
                return DbHelper.Terminals.Values;

            if (DbHelper.UpdateTerminals(Common.UserName, 1, int.MaxValue, true))
                return DbHelper.Terminals.Values;
            else
                return null;
        }

        protected IEnumerable<Group> GetAllGroups(bool update)
        {
            if (!update && DbHelper.Groups != null)
                return DbHelper.Groups;

            if (DbHelper.UpdateAllGroups(Common.UserName))
                return DbHelper.Groups;
            else
                return null;
        }
    }
}