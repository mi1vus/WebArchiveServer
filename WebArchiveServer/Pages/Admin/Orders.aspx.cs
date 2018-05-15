using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.ModelBinding;
//using WebArchiveServer.Models;
//using WebArchiveServer.Models.Repository;

namespace WebArchiveServer.Pages.Admin
{
    public partial class Orders : System.Web.UI.Page
    {
        //private Repository repository = new Repository();
        //public static List<Order> orders = new List<Order>() {
        //        new Order
        //        {
        //            City = "Moscow",
        //            Name = "Diablo",
        //            Dispatched = false,
        //            GiftWrap = true,
        //            Line1 = "111111111111",
        //            Line2 = "222222222222",
        //            Line3 = "333333333333",
        //            OrderId = 1,
        //            OrderLines = new List<OrderLine>()
        //            {
        //                new OrderLine()
        //                {
        //                    Game = Listing.games[0],
        //                    OrderLineId = 1,
        //                    Quantity = 2
        //                }
        //            }
        //        },
        //        new Order
        //        {
        //            Name = "Lineage",
        //            City = "Moscow",
        //            Dispatched = false,
        //            GiftWrap = true,
        //            Line1 = "1311111111111",
        //            Line2 = "2222222222222",
        //            Line3 = "3133333333333",
        //            OrderId = 2,
        //            OrderLines = new List<OrderLine>()
        //            {
        //                new OrderLine()
        //                {
        //                    Game = Listing.games[1],
        //                    OrderLineId = 2,
        //                    Quantity = 2
        //                }
        //            }                }};

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                int dispatchID;
                if (int.TryParse(Request.Form["dispatch"], out dispatchID))
                {
                    //Order myOrder = /*repository.Orders*/orders.Where(o => o.OrderId == dispatchID).FirstOrDefault();
                    //if (myOrder != null)
                    //{
                    //    myOrder.Dispatched = true;
                    //    //repository.SaveOrder(myOrder);
                    //}
                }
            }
        }

        public IEnumerable<int/*Order*/> GetOrders([Control] bool showDispatched)
        {
            //if (showDispatched)
            //{
            //    return orders/*repository.Orders*/;
            //}
            //else
            //{
            //    return /*repository.Orders*/orders.Where(o => !o.Dispatched);
            //}
            return null;
        }

        public decimal Total(IEnumerable<int/*OrderLine*/> orderLines)
        {
            decimal total = 0;
            //foreach (OrderLine ol in orderLines)
            //{
            //    total += ol.Game.Price * ol.Quantity;
            //}
            return total;
        }
    }
}