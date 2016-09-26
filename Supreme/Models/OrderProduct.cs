using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class OrderProduct
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int orderId { set; get; }

        [Required]
        public int productId { set; get; }

        public virtual Product product { set; get; }

        [Required]
        public int quantity { set; get; }

        [Required]
        public double price { set; get; }

        public string sku { set; get; }
    }
}