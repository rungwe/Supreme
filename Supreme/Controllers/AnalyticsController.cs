using Supreme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Supreme.Controllers
{
    public class AnalyticsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        [Route("api/getorderanalytics")]
        public IQueryable<orderAnalytics> GetOrderAnalytics()
        {
            var orders = from b in db.Orders
                         select new orderAnalytics
                         {
                             
                             price = b.price,
                             date = b.date,
                             status = b.status,
                            warehouseLocation = b.warehouseLocation
                         };

            return orders;
        }

    }
}
