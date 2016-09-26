using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme
{
    public class ProductPricingDTO
    {
        public int id { set; get; }

        public int productId { set; get; }

        public int customerId { set; get; }

        public double amount { set; get; }

        public string sku { set; get; }

        public string description { set; get; }
    }
}