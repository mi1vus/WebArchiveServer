using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.UI;
using WebApplication6.Models;
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
            UpdateProducts();
        }

        private List<Product> products = new List<Product>();

        private void UpdateProducts()
        {
            MySqlConnection conn = new MySqlConnection(connStrTest);
            conn.Open();
            string sql = "SELECT id,name,category,price FROM products";
            MySqlCommand command = new MySqlCommand(sql, conn);
            var dataReader = command.ExecuteReader();
            products.Clear();
            while (dataReader.Read())
            {
                products.Add(new Product()
                {
                    Id = dataReader.GetInt32(0),
                    Name = dataReader.GetString(1),
                    Category = dataReader.GetString(2),
                    Price = dataReader.GetDecimal(3)
                });
            }
            conn.Close();
        }

        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            UpdateProducts();
            return products;
        }

        [HttpGet]

        public IHttpActionResult GetProduct(int id)
        {
            var u = User;
            UpdateProducts();
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet]
        public IEnumerable<Parameter> GetParameters(string HaspId)
        {
            var u = User;
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
        public int UpdateSaveDate(int TId, int ParId)
        {
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
            decimal Summ
            )
        {
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