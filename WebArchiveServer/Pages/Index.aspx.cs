using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Security;
using MySql.Data.MySqlClient;
using WebArchiveServer.Models;

namespace WebArchiveServer.Pages
{
    public partial class Index : System.Web.UI.Page
    {
        private int pageSize = 2;

        protected int CurrentPageTerminal
        {
            get
            {
                int page;
                page = int.TryParse(Request.QueryString["pageT"], out page) ? page : 1;
                return page > MaxPageTerminal ? MaxPageTerminal : page <= 0 ? 1 : page;
            }
        }
        protected int CurrentPageOrder
        {
            get
            {
                int page;
                page = int.TryParse(Request.QueryString["pageO"], out page) ? page : 1;
                return page > MaxPageOrder ? MaxPageOrder : page <= 0 ? 1 : page;
            }
        }

        protected int MaxPageTerminal
        {
            get
            {
                decimal count = TerminalsCount();
                if (count <= 0)
                    return 1;

                return (int)Math.Ceiling(count / pageSize);
            }
        }
        protected int MaxPageOrder
        {
            get
            {
                int id;
                if (int.TryParse(Request.QueryString["idT"], out id))
                {
                    decimal count = OrdersCount(id);
                    if (count <= 0)
                        return 1;

                    return (int) Math.Ceiling(count / pageSize);
                }
                else
                return 1;
        }
        }
        
        // строка подключения к БД
        private static string connStr = "server=localhost;user=MYSQL;database=terminal_archive;password=tt2QeYy2pcjNyBm6AENp;";
        private static string connStrTest = "server=localhost;user=MYSQL;database=products;password=tt2QeYy2pcjNyBm6AENp;";
        public Index()
        {
            //UpdateTerminals();
        }

        private static Dictionary<int, Terminal> terminals = new Dictionary<int, Terminal>();

        private int TerminalsCount()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = "SELECT COUNT(id) FROM terminal_archive.terminals;";
            MySqlCommand countCommand = new MySqlCommand(sql, conn);
            try
            {
                var dataReader = countCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    return dataReader.GetInt32(0);
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
            return -1;
        }
        private int OrdersCount(int idTerminal)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = $@"SELECT COUNT(id) FROM terminal_archive.orders AS o WHERE o.id_terminal = {idTerminal};";
            MySqlCommand countCommand = new MySqlCommand(sql, conn);
            try
            {
                var dataReader = countCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    return dataReader.GetInt32(0);
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
            return -1;
        }

        private void UpdateTerminals()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql =
                $@"SELECT t.`id`, t.`name`, g.`id`,  g.`name` AS `группа` ,  t.`address` , t.`hasp_id`
/*, p.id AS `id параметра`, p.name AS `имя параметра` , p.path AS `путь параметра`, tp.value AS `значение параметра`,
 tp.last_edit_date, tp.save_date*/
 FROM terminal_archive.terminals AS t
 LEFT JOIN terminal_archive.terminal_groups AS g ON t.id_group = g.id
/*LEFT JOIN terminal_archive.terminal_parameters AS tp ON t.id = tp.id_terminal
LEFT JOIN terminal_archive.parameters AS p ON tp.id_parameter = p.id*/
ORDER BY t.id asc LIMIT {(CurrentPageTerminal - 1)*pageSize},{pageSize};";

            MySqlCommand command = new MySqlCommand(sql, conn);
            terminals.Clear();
            var dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                int id = dataReader.GetInt32(0);
                terminals[id] =  new Terminal()
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
            conn.Close();
        }

        private void UpdateTerminalOrders(int idTerminal)
        {
            if (!terminals.Any())
                UpdateTerminals();
            MySqlConnection conn = new MySqlConnection(connStr);
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
ORDER BY o.id desc LIMIT {(CurrentPageOrder - 1)*pageSize},{pageSize};";
            
            MySqlCommand command = new MySqlCommand(sql, conn);
            var dataReader = command.ExecuteReader();
            Dictionary<int, Order> orders = new Dictionary<int, Order>();
            while (dataReader.Read())
            {
                int orderId = dataReader.GetInt32(0);
                if (!orders.ContainsKey(orderId))
                {
                    var order = new Order()
                    {
                        Id = orderId,
                        RNN = dataReader.GetString(1),
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
            terminals[idTerminal].Orders = orders;
            dataReader.Close();
            conn.Close();
        }

        private void UpdateTerminalParameters(int idTerminal)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql =
$@"SELECT p.id AS `id параметра`, p.name AS `имя параметра`, p.path AS `путь параметра` ,tp.value AS `значение параметра`, 
tp.last_edit_date, tp.save_date
FROM terminal_archive.terminals AS t
LEFT JOIN terminal_archive.terminal_parameters AS tp ON t.id = tp.id_terminal
LEFT JOIN terminal_archive.parameters AS p ON tp.id_parameter = p.id
WHERE t.id = {idTerminal} /*tp.save_date < tp.last_edit_date*/
ORDER BY p.id desc;";
            MySqlCommand command = new MySqlCommand(sql, conn);
            var dataReader = command.ExecuteReader();
            List<Parameter> parameters = new List<Parameter>();
            while (dataReader.Read())
            {
                int parameterId = dataReader.GetInt32(0);
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
            terminals[idTerminal].Parameters = parameters;
            dataReader.Close();
            conn.Close();
        }

        protected IEnumerable<Terminal> GetAllTerminals()
        {
            UpdateTerminals();
            return terminals.Values;
        }

        protected Terminal GetTerminalOrders(int idTerminal)
        {
            var u = User;
            UpdateTerminalOrders(idTerminal);
            //var terminal = terminals.FirstOrDefault((p) => p.Key == idTerminal).Value.ToArray();
            return terminals.ContainsKey(idTerminal) ? terminals[idTerminal] : null;
        }

        protected Terminal GetTerminalParameters(int idTerminalP)
        {
            var u = User;
            UpdateTerminalParameters(idTerminalP);
            //var terminal = terminals.FirstOrDefault(p => p.Key == idTerminalP).Value;
            return terminals.ContainsKey(idTerminalP) ? terminals[idTerminalP] : null;
        }

        protected IEnumerable<Parameter> GetParameters(string HaspId, string User, string Pass)
        {
            if (string.IsNullOrWhiteSpace(User) || string.IsNullOrWhiteSpace(Pass)
                || !FormsAuthentication.Authenticate(User, Pass))
                return null;

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql =
$@"SELECT t.`id`, t.`hasp_id`,  g.`name` AS `группа` ,  t.`address` , t.`name`, 
p.id AS `id параметра`, p.name AS `имя параметра` ,p.path AS `путь параметра`, tp.value AS `значение параметра`, 
tp.last_edit_date, tp.save_date
FROM terminal_archive.terminals AS t
LEFT JOIN terminal_archive.terminal_groups AS g ON t.id_group = g.id
LEFT JOIN terminal_archive.terminal_parameters AS tp ON t.id = tp.id_terminal
LEFT JOIN terminal_archive.parameters AS p ON tp.id_parameter = p.id
WHERE tp.save_date < tp.last_edit_date AND t.hasp_id = '{HaspId}'
ORDER BY t.id asc; ";
            MySqlCommand command = new MySqlCommand(sql, conn);
            var dataReader = command.ExecuteReader();
            List<Parameter> parameters = new List<Parameter>();
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
            conn.Close();
            return parameters;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                FormsAuthentication.SignOut();
            }
        }
    }
}