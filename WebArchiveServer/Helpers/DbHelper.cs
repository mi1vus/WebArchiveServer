using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using MySql.Data.MySqlClient;
using WebArchiveServer.Models;

namespace WebArchiveServer.Helpers
{
    public static class DbHelper
    {
        // строка подключения к БД
        private static readonly string ConnStr /*= "server=localhost;user=MYSQL;database=terminal_archive;password=tt2QeYy2pcjNyBm6AENp;"*/;
        private static string _connStrTest = "server=localhost;user=MYSQL;database=products;password=tt2QeYy2pcjNyBm6AENp;";

        public static Dictionary<int, Terminal> Terminals = new Dictionary<int, Terminal>();

        static DbHelper()
        {
            var rootWebConfig =
            System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/MyWebSiteRoot");
            if (rootWebConfig.AppSettings.Settings.Count <= 0) return;
            var customSetting =
                rootWebConfig.AppSettings.Settings["connStr"];
            if (customSetting != null)
                ConnStr = customSetting.Value;
        }

        public static bool IsAuthorizeUser(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var users = 0;
            var conn = new MySqlConnection(ConnStr);
            try
            {
                var sql = "";
                using (var md5Hash = MD5.Create())
                {
                    var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                    var sBuilder = new StringBuilder();
                    foreach (var t in data)
                    {
                        sBuilder.Append(t.ToString("x2"));
                    }
                    var hash = sBuilder.ToString();

                    sql =
                        $"SELECT COUNT(id) FROM terminal_archive.users AS u WHERE u.name = '{name}' AND u.pass = '{hash}';";
                }
                conn.Open();
                var countCommand = new MySqlCommand(sql, conn);

                var dataReader = countCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    users = dataReader.GetInt32(0);
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }
            return users > 0;
        }

        public static bool UserInRole(string name, string role)
        {
            var users = 0;
            if (name != null && role != null)
            {
                string sql =
$@"SELECT COUNT(u.id) FROM terminal_archive.users AS u 
LEFT JOIN terminal_archive.user_groups AS ug ON u.id_group = ug.id
WHERE u.name = '{name}' AND ug.name = '{role}'; ";
                var conn = new MySqlConnection(ConnStr);
                conn.Open();
                var countCommand = new MySqlCommand(sql, conn);
                try
                {
                    var dataReader = countCommand.ExecuteReader();
                    while (dataReader.Read())
                    {
                        users = dataReader.GetInt32(0);
                    }
                    dataReader.Close();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    conn.Close();
                }
            }
            return users > 0;
        }

        public static int TerminalsCount()
        {
            var result = 0;
            var conn = new MySqlConnection(ConnStr);
            try
            {
                conn.Open();
                var sql = "SELECT COUNT(id) FROM terminal_archive.terminals;";
                var countCommand = new MySqlCommand(sql, conn);

                var dataReader = countCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    result = dataReader.GetInt32(0);
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                result = -1;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public static int OrdersCount(int idTerminal)
        {
            var result = 0;
            var conn = new MySqlConnection(ConnStr);
            try
            {
                conn.Open();
                string sql = $@"SELECT COUNT(id) FROM terminal_archive.orders AS o WHERE o.id_terminal = {idTerminal};";
                var countCommand = new MySqlCommand(sql, conn);

                var dataReader = countCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    result =  dataReader.GetInt32(0);
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                result = -1;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public static bool UpdateTerminals(int currentPageTerminal, int pageSize)
        {
            bool result;
            var conn = new MySqlConnection(ConnStr);
            try {
                conn.Open();
                string sql =
                $@"SELECT t.`id`, t.`name`, g.`id`,  g.`name` AS `группа` ,  t.`address` , t.`hasp_id`
/*, p.id AS `id параметра`, p.name AS `имя параметра` , p.path AS `путь параметра`, tp.value AS `значение параметра`,
 tp.last_edit_date, tp.save_date*/
 FROM terminal_archive.terminals AS t
 LEFT JOIN terminal_archive.terminal_groups AS g ON t.id_group = g.id
/*LEFT JOIN terminal_archive.terminal_parameters AS tp ON t.id = tp.id_terminal
LEFT JOIN terminal_archive.parameters AS p ON tp.id_parameter = p.id*/
ORDER BY t.id asc LIMIT {(currentPageTerminal - 1) * pageSize},{pageSize};";

                var command = new MySqlCommand(sql, conn);
                Terminals.Clear();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var id = dataReader.GetInt32(0);
                    Terminals[id] = new Terminal()
                    {
                        Id = id,
                        Name = dataReader.GetString(1),
                        IdGroup = dataReader.GetInt32(2),
                        GroupName = dataReader.GetString(3),
                        Address = dataReader.GetString(4),
                        IdHasp = dataReader.GetString(5),
                        //Orders = new List<Order>()
                    };
                }
                dataReader.Close();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public static bool UpdateTerminalOrders(int idTerminal, int currentPageTerminal, int currentPageOrder, int pageSize)
        {
            if (!Terminals.Any())
                UpdateTerminals(currentPageTerminal, pageSize);
            bool result;
            var conn = new MySqlConnection(ConnStr);
            try
            {
                conn.Open();
                string sql =
                $@"SELECT o.`id`, `RNN`, s.id, s.name AS `состояние`, t.id, t.`name` AS `терминал` ,
d.id, d.description AS `доп. параметр`, od.value AS `значение`,
f.id, f.`name` AS `топливо` , p.id, p.`name` AS `оплата` , o.id_pump AS `колонка`,  
`pre_price` ,  `price` ,  `pre_quantity` ,  `quantity` ,  `pre_summ` ,  `summ` FROM terminal_archive.orders AS o
LEFT JOIN terminal_archive.order_fuels AS f ON o.id_fuel = f.id
LEFT JOIN terminal_archive.order_payment_types AS p ON o.id_payment = p.id
LEFT JOIN terminal_archive.terminals AS t ON o.id_terminal = t.id
LEFT JOIN terminal_archive.order_states AS s ON o.id_state = s.id
LEFT JOIN terminal_archive.order_details AS od ON o.id = od.id_order
LEFT JOIN terminal_archive.details AS d ON od.id_detail = d.id
WHERE t.id = {idTerminal}
ORDER BY o.id desc LIMIT {(currentPageOrder - 1) * pageSize},{pageSize};";

                var command = new MySqlCommand(sql, conn);
                var dataReader = command.ExecuteReader();
                var orders = new Dictionary<int, Order>();
                while (dataReader.Read())
                {
                    var orderId = dataReader.GetInt32(0);
                    if (!orders.ContainsKey(orderId))
                    {
                        var order = new Order()
                        {
                            Id = orderId,
                            Rnn = dataReader.GetString(1),
                            IdState = dataReader.GetInt32(2),
                            StateName = dataReader.GetString(3),
                            IdTerminal = dataReader.GetInt32(4),
                            TerminalName = dataReader.GetString(5),
                            AdditionalParameters = new List<AdditionalParameter>(),
                            IdFuel = dataReader.GetInt32(9),
                            FuelName = dataReader.GetString(10),
                            IdPayment = dataReader.GetInt32(11),
                            PaymentName = dataReader.GetString(12),
                            IdPump = dataReader.GetInt32(13),
                            PrePrice = dataReader.GetDecimal(13),
                            Price = dataReader.GetDecimal(13),
                            PreQuantity = dataReader.GetDecimal(13),
                            Quantity = dataReader.GetDecimal(13),
                            PreSumm = dataReader.GetDecimal(13),
                            Summ = dataReader.GetDecimal(13),
                        };
                        orders[orderId] = order;
                    }
                    if (!dataReader.IsDBNull(6))
                    {
                        var additionalParameter = new AdditionalParameter();
                        additionalParameter.Id = dataReader.GetInt32(6);
                        additionalParameter.IdOrder = orderId;
                        additionalParameter.Name = dataReader.GetString(7);
                        additionalParameter.Value = dataReader.GetString(8);
                        orders[orderId].AdditionalParameters.Add(additionalParameter);
                    }
                }
                Terminals[idTerminal].Orders = orders;
                dataReader.Close();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public static bool UpdateTerminalParameters(int idTerminal)
        {
            bool result;
            var conn = new MySqlConnection(ConnStr);
            try
            {
                conn.Open();
                string sql =
                $@"SELECT p.id AS `id параметра`, p.name AS `имя параметра`, p.path AS `путь параметра` ,tp.value AS `значение параметра`, 
tp.last_edit_date, tp.save_date
FROM terminal_archive.terminals AS t
LEFT JOIN terminal_archive.terminal_parameters AS tp ON t.id = tp.id_terminal
LEFT JOIN terminal_archive.parameters AS p ON tp.id_parameter = p.id
WHERE t.id = {idTerminal} /*tp.save_date < tp.last_edit_date*/
ORDER BY p.id desc;";

                var command = new MySqlCommand(sql, conn);
                var dataReader = command.ExecuteReader();
                var parameters = new List<Parameter>();
                while (dataReader.Read())
                {
                    var parameterId = dataReader.GetInt32(0);
                    parameters.Add(new Parameter()
                    {
                        Id = parameterId,
                        TId = idTerminal,
                        Name = dataReader.GetString(1),
                        Path = dataReader.GetString(2),
                        Value = dataReader.GetString(3),
                        LastEditTime = DateTime.ParseExact(dataReader.GetString(4), "dd.MM.yyyy HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture),
                        SaveTime = DateTime.ParseExact(dataReader.GetString(5), "dd.MM.yyyy HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture),
                    });
                }
                Terminals[idTerminal].Parameters = parameters;
                dataReader.Close();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public static bool AddHistory(
            string haspId, string rrn,
            string trace, string msg, int? errorLevel,
            string user, string pass
        )
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass)
                || !IsAuthorizeUser(user, pass) || !UserInRole(user, "Admin"))
                return false;

            var conn = new MySqlConnection(ConnStr);
            try
            {
                conn.Open();

                string selectSql =
$@"SELECT t.id FROM terminal_archive.terminals AS t
WHERE t.hasp_id = '{haspId}';";
                var selectCommand = new MySqlCommand(selectSql, conn);
                var reader = selectCommand.ExecuteReader();
                reader.Read();
                var terminal = reader.GetInt32(0);
                reader.Close();

                selectSql =
$@"SELECT o.id, o.id_state FROM terminal_archive.orders AS o 
WHERE o.RNN = '{rrn}';";

                selectCommand = new MySqlCommand(selectSql, conn);
                reader = selectCommand.ExecuteReader();
                reader.Read();
                var order = -1;
                if(!string.IsNullOrWhiteSpace(rrn) && !reader.IsDBNull(0))
                    order = reader.GetInt32(0);
                var state = -1;
                if (errorLevel != null && errorLevel > 0)
                        state = 1000 + errorLevel.Value;
                else if (!string.IsNullOrWhiteSpace(rrn) && !reader.IsDBNull(1))
                        state = reader.GetInt32(1);

                reader.Close();

                string addSql = string.Format(
@"INSERT INTO
`history` (`id_terminal`{0}{1}{2},`msg`)
VALUES
('{3}'{4}{5}{6},'{7}');", 
order < 0 ? "" : ",`id_order`",
state < 0 ? "" : ",`id_state`",
string.IsNullOrWhiteSpace(trace) ? "" : ",`trace`",
terminal,
order < 0 ? "" : $",'{order}'",
state < 0 ? "" : $",'{state}'",
string.IsNullOrWhiteSpace(trace) ? "" : $",'{trace}'",
msg);

                var addCommand = new MySqlCommand(addSql, conn);
                var result = addCommand.ExecuteNonQuery();
                return result > 0;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public static IEnumerable<Parameter> GetParameters(string haspId, string user, string pass)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass)
                || !IsAuthorizeUser(user, pass) || !UserInRole(user, "Admin"))
                return null;

            List<Parameter> parameters = new List<Parameter>();
            MySqlConnection conn = new MySqlConnection(ConnStr);
            try
            {
                conn.Open();
                string sql =
$@"SELECT t.`id`, t.`hasp_id`,  g.`name` AS `группа` ,  t.`address` , t.`name`, 
p.id AS `id параметра`, p.name AS `имя параметра` ,p.path AS `путь параметра`, tp.value AS `значение параметра`, 
tp.last_edit_date, tp.save_date
FROM terminal_archive.terminals AS t
LEFT JOIN terminal_archive.terminal_groups AS g ON t.id_group = g.id
LEFT JOIN terminal_archive.terminal_parameters AS tp ON t.id = tp.id_terminal
LEFT JOIN terminal_archive.parameters AS p ON tp.id_parameter = p.id
WHERE tp.save_date < tp.last_edit_date AND t.hasp_id = '{haspId}'
ORDER BY t.id asc; ";
                MySqlCommand command = new MySqlCommand(sql, conn);
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    parameters.Add(new Parameter()
                    {
                        Id = dataReader.GetInt32(5),
                        TId = dataReader.GetInt32(0),
                        Name = dataReader.GetString(6),
                        Path = dataReader.GetString(7),
                        Value = dataReader.GetString(8)
                    });
                }
            }
            catch (Exception ex)
            {
                parameters = null;
            }
            finally
            {
                conn.Close();
            }
            return parameters;
        }

        public static int UpdateSaveDate(int id, int parId, string user, string pass)
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass)
                || !IsAuthorizeUser(user, pass) || !UserInRole(user, "Admin"))
                return -1;

            var result = -1;
            var conn = new MySqlConnection(ConnStr);
            try
            {
                conn.Open();

                var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
                numberFormatInfo.NumberGroupSeparator = "";
                numberFormatInfo.NumberDecimalSeparator = ".";

                var now = DateTime.Now.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss");

                string updateSql =
$@"UPDATE terminal_archive.terminal_parameters 
SET save_date = '{now}' 
WHERE id_terminal = '{id}' AND id_parameter = '{parId}';";

                var updateCommand = new MySqlCommand(updateSql, conn);
                result = updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public static bool AddNewOrder(
            string rrn,//
            string haspId,
            int fuel,
            int pump,
            int payment,
            int state,
            decimal prePrice,
            decimal price,
            decimal preQuantity,
            decimal quantity,
            decimal preSumm,
            decimal summ,
            string user, string pass
            )
        {
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass)
                || !IsAuthorizeUser(user, pass) || !UserInRole(user, "Admin"))
                return false;

            int result = 0;
            var conn = new MySqlConnection(ConnStr);
            try
            {
                conn.Open();
                var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
                numberFormatInfo.NumberGroupSeparator = "";
                numberFormatInfo.NumberDecimalSeparator = ".";

                string selectSql =
$@"SELECT count(o.id), t.id FROM terminal_archive.terminals AS t
LEFT JOIN terminal_archive.orders AS o   ON o.id_terminal = t.id 
WHERE t.hasp_id = '{haspId}' AND o.RNN = '{rrn}';";
  //              o.id_terminal = {terminal} AND o.RNN = '';";

                var selectCommand = new MySqlCommand(selectSql, conn);
                var reader = selectCommand.ExecuteReader();
                reader.Read();
                var orders = reader.GetInt32(0);
                var terminal = -1;
                if (orders > 0)
                {
                    terminal = reader.GetInt32(1);
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    selectSql =
$@"SELECT t.id FROM terminal_archive.terminals AS t
WHERE t.hasp_id = '{haspId}';";
                    selectCommand = new MySqlCommand(selectSql, conn);
                    reader = selectCommand.ExecuteReader();
                    reader.Read();
                    terminal = reader.GetInt32(0);
                    reader.Close();
                }
                var addSql = String.Empty;
                if (orders > 0)
                {
                    addSql =
$@"UPDATE `orders` AS o SET
`id_state`={state},
`pre_price`={prePrice.ToString(numberFormatInfo)},
`price`={price.ToString(numberFormatInfo)},
`pre_quantity`={preQuantity.ToString(numberFormatInfo)},
`quantity`={quantity.ToString(numberFormatInfo)},
`pre_summ`={preSumm.ToString(numberFormatInfo)},
`summ`={summ.ToString(numberFormatInfo)}
WHERE 
o.id_terminal = {terminal} AND o.RNN = '{rrn}';";
                }
                else
                {
                    addSql =
$@"INSERT INTO
`orders` (`id_terminal`,`RNN`,`id_fuel`,`id_pump`,`id_payment`,`id_state`,`pre_price`,`price`,`pre_quantity`,`quantity`,`pre_summ`,`summ`)
VALUES
({terminal},'{rrn}',{fuel},{pump},{payment},{state},{prePrice.ToString(numberFormatInfo)},{price.ToString(numberFormatInfo)},{preQuantity.ToString(numberFormatInfo)},{quantity.ToString(numberFormatInfo)},{preSumm.ToString(numberFormatInfo)},{summ.ToString(numberFormatInfo)})";
                }
                var addCommand = new MySqlCommand(addSql, conn);

                result = addCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }
            return result > 0;
        }
    }
}