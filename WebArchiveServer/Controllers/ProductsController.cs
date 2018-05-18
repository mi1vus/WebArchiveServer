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
using WebArchiveServer.Helpers;

namespace ProductsApp.Controllers
{
    public class TerminalsController : ApiController
    {
        public TerminalsController()
        {
        }

        [HttpGet]
        [HttpPost]
        public IEnumerable<Parameter> GetParameters(string HaspId, string User, string Pass)
        {
            return DbHelper.GetParameters(HaspId, User, Pass);
        }

        [HttpGet]
        [HttpPost]
        public int UpdateSaveDate(int TId, int ParId, string User, string Pass)
        {
            return DbHelper.UpdateSaveDate(TId, ParId, User, Pass);
        }

        [HttpGet]
        [HttpPost]
        public bool AddNewOrder(
            string RRN,
            string HaspId,
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
            return DbHelper.AddNewOrder(RRN, HaspId, Fuel, Pump, Payment, State, PrePrice, Price, PreQuantity, Quantity, PreSumm, Summ, User,  Pass);
        }
    }
}