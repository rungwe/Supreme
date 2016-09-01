using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Return
    {
        [Key]
        public int id { set; get; }

        public int orderId { set; get; }

        public virtual Order order { set; get; }

        public int stockManagerId { set; get; }

        public virtual StockManager stockManager { set; get; }

        public int driverId {set;get;}

        public virtual Driver driver { set; get; }

        public string status { set; get; }

        public string reason { set; get; }
    }
}