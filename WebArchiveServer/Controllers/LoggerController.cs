using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using MySql.Data.MySqlClient;
using WebArchiveServer.Helpers;
using static System.Web.Security.FormsAuthentication;

namespace WebTerminalServer.Controllers
{
    public class HistoryController : ApiController
    {
        public HistoryController()
        {
        }

        [HttpPost]
        public bool AddHistory(
            string HaspId, string RRN,
            string Trace, string Msg, int? ErrorLevel,
            string User, string Pass
        )
        {
            
            return DbHelper.AddHistory(HaspId, RRN,Trace , Msg, ErrorLevel, User, Pass);
        }

    }
}
