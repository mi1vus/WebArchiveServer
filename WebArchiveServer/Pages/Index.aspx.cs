using System;
using System.Collections.Generic;
using WebArchiveServer.Helpers;
using WebArchiveServer.Models;

namespace WebArchiveServer.Pages
{
    public partial class Index : System.Web.UI.Page
    {
        protected int CurrentPageTerminal
        {
            get
            {
                int page;
                page = int.TryParse(Request.QueryString["pageT"], out page) ? page : 1;
                return page > MaxPageTerminal ? MaxPageTerminal : page <= 0 ? 1 : page;
            }
        }

        protected int MaxPageTerminal
        {
            get
            {
                decimal count = DbHelper.TerminalsCount(Common.UserName);
                if (count <= 0)
                    return 1;

                return (int)Math.Ceiling(count / Common.PageSize);
            }
        }
        
        public Index()
        {
            //UserName = userName;
            //UpdateTerminals();
        }

        protected IEnumerable<Terminal> GetTerminals()
        {
            if (DbHelper.UpdateTerminals(Common.UserName, CurrentPageTerminal, Common.PageSize))
                return DbHelper.Terminals.Values;
            else
                return null;
        }

//        protected IEnumerable<Parameter> GetParameters(string HaspId, string user, string pass)
//        {
//            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass)
//                || !FormsAuthentication.Authenticate(user, pass))
//                return null;

////            MySqlConnection conn = new MySqlConnection(connStr);
////            conn.Open();
////            string sql =
////$@"SELECT t.`id`, t.`hasp_id`,  g.`name` AS `группа` ,  t.`address` , t.`name`, 
////p.id AS `id параметра`, p.name AS `имя параметра` ,p.path AS `путь параметра`, tp.value AS `значение параметра`, 
////tp.last_edit_date, tp.save_date
////FROM terminal_archive.terminals AS t
////LEFT JOIN terminal_archive.terminal_groups AS g ON t.id_group = g.id
////LEFT JOIN terminal_archive.terminal_parameters AS tp ON t.id = tp.id_terminal
////LEFT JOIN terminal_archive.parameters AS p ON tp.id_parameter = p.id
////WHERE tp.save_date < tp.last_edit_date AND t.hasp_id = '{HaspId}'
////ORDER BY t.id asc; ";
////            MySqlCommand command = new MySqlCommand(sql, conn);
////            var dataReader = command.ExecuteReader();
//            var parameters = new List<Parameter>();
////            while (dataReader.HasRows && dataReader.Read())
////            {
////                parameters.Add(new Parameter()
////                {
////                    Id = dataReader.GetInt32(5),
////                    TId = dataReader.GetInt32(0),
////                    Name = dataReader.GetString(6),
////                    Path = dataReader.GetString(7),
////                    Value = dataReader.GetString(8)
////                });
////            }
////            conn.Close();
//            return parameters;
//        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}