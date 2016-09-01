using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class OrderItem
    {
        public int productId { set; get; }
        public int quantity { set; get; }
    }
}