using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class OrderProductDTO
    {
        public int productId { set; get; }
        public string productName { set; get; }
        public string productDescription { set; get; }
        public int quantity { set; get; }
        public double price { set; get; }

    }

}