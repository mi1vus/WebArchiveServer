﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Security;
using System.Web.UI;
using WebArchiveServer.Models;
using MySql.Data.MySqlClient;

namespace ProductsApp.Controllers
{
    public class TerminalsController : ApiController
    {
        // строка подключения к БД
        private static string connStr = "server=localhost;user=MYSQL;database=terminal_archive;password=tt2QeYy2pcjNyBm6AENp;";
        private static string connStrTest = "server=localhost;user=MYSQL;database=products;password=tt2QeYy2pcjNyBm6AENp;";
        public TerminalsController()
        {
            UpdateTerminals();
        }

        private static Dictionary<int,Terminal> terminals = new Dictionary<int, Terminal>();

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
ORDER BY t.id asc;";
            MySqlCommand command = new MySqlCommand(sql, conn);
            var dataReader = command.ExecuteReader();
            terminals.Clear();
            while (dataReader.Read())
            {
                int id = dataReader.GetInt32(0);
                terminals.Add(id, new Terminal()
                {
                    Id = id,
                    Name = dataReader.GetString(1),
                    IdGroup = dataReader.GetInt32(2),
                    GroupName = dataReader.GetString(3),
                    Address = dataReader.GetString(4),
                    IdHasp = dataReader.GetString(5),
                    //Orders = new List<Order>()
                });
            }
            dataReader.Close();
            conn.Close();
        }

        private void UpdateTerminalOrders(int idTerminal, int limit)
        {
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
ORDER BY o.id desc LIMIT {limit};";
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

        [HttpGet]
        [HttpPost]
        public IEnumerable<Terminal> GetAllTerminals()
        {
            UpdateTerminals();
            return terminals.Values;
        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult GetTerminalOrders(int idTerminal)
        {
            var u = User;
            UpdateTerminalOrders(idTerminal,10);
            var terminal = terminals.FirstOrDefault((p) => p.Key == idTerminal).Value;
            if (terminal == null)
            {
                return NotFound();
            }
            return Ok(terminal);
        }

        [HttpGet]
        [HttpPost]
        public IHttpActionResult GetTerminalParameters(int idTerminalP)
        {
            var u = User;
            UpdateTerminalParameters(idTerminalP);
            var terminal = terminals.FirstOrDefault(p => p.Key == idTerminalP).Value;
            if (terminal == null)
            {
                return NotFound();
            }
            return Ok(terminal);
        }

        [HttpGet]
        [HttpPost]
        public IEnumerable<Parameter> GetParameters(string HaspId, string User, string Pass)
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

        [HttpGet]
        [HttpPost]
        public int UpdateSaveDate(int TId, int ParId, string User, string Pass)
        {
            if (string.IsNullOrWhiteSpace(User) || string.IsNullOrWhiteSpace(Pass)
                || !FormsAuthentication.Authenticate(User, Pass))
                return -1;

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
            numberFormatInfo.NumberGroupSeparator = "";
            numberFormatInfo.NumberDecimalSeparator = ".";

            var now = DateTime.Now.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss");

            string updateSql =
$@"UPDATE terminal_archive.terminal_parameters 
SET save_date = '{now}' 
WHERE id_terminal = '{TId}' AND id_parameter = '{ParId}';";

            MySqlCommand updateCommand = new MySqlCommand(updateSql, conn);
            try
            {
                var result = updateCommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
            }
            return -1;
        }

        [HttpGet]
        [HttpPost]
        public int AddNewOrder(
            string RRN,
            int Terminal,
            int Fuel,
            int Pump,
            int Payment,
            int State,
            decimal PrePrice,
            decimal Price,
            decimal PreQuantity,
            decimal Quantity,
            decimal PreSumm,
            decimal Summ, 
            string User, string Pass
            )
        {
            if (string.IsNullOrWhiteSpace(User) || string.IsNullOrWhiteSpace(Pass)
                || !FormsAuthentication.Authenticate(User, Pass))
                return -1;

            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
            numberFormatInfo.NumberGroupSeparator = "";
            numberFormatInfo.NumberDecimalSeparator = ".";

            string selectSql =
$@"SELECT count(id) FROM terminal_archive.orders AS o WHERE 
o.id_terminal = {Terminal} AND o.RNN = '{RRN}';";

            MySqlCommand selectCommand = new MySqlCommand(selectSql, conn);
            var reader = selectCommand.ExecuteReader();
            reader.Read();
            int  orders = reader.GetInt32(0);
            reader.Close();
            string addSql = String.Empty;
            if (orders > 0)
            {
                addSql =
    $@"UPDATE `orders` AS o SET
    `id_state`={State},
    `pre_price`={PrePrice.ToString(numberFormatInfo)},
    `price`={Price.ToString(numberFormatInfo)},
    `pre_quantity`={PreQuantity.ToString(numberFormatInfo)},
    `quantity`={Quantity.ToString(numberFormatInfo)},
    `pre_summ`={PreSumm.ToString(numberFormatInfo)},
    `summ`={Summ.ToString(numberFormatInfo)}
    WHERE 
    o.id_terminal = {Terminal} AND o.RNN = '{RRN}';";
            }
            else
            {
                addSql =
    $@"INSERT INTO
    `orders` (`id_terminal`,`RNN`,`id_fuel`,`id_pump`,`id_payment`,`id_state`,`pre_price`,`price`,`pre_quantity`,`quantity`,`pre_summ`,`summ`)
    VALUES
    ({Terminal},'{RRN}',{Fuel},{Pump},{Payment},{State},{PrePrice.ToString(numberFormatInfo)},{Price.ToString(numberFormatInfo)},{PreQuantity.ToString(numberFormatInfo)},{Quantity.ToString(numberFormatInfo)},{PreSumm.ToString(numberFormatInfo)},{Summ.ToString(numberFormatInfo)})";
            }
            MySqlCommand addCommand = new MySqlCommand(addSql, conn);
            try
            {
                var result = addCommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
            }
            return -1;
        }
    }
}