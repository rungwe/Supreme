using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class ProductPriceCreateDTO
    {
        
        

        public int productId { set; get; }

        public int customerId { set; get; }

        public double amount { set; get; }

        public string description { get; set; }
    }
}