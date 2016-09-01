using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class ProductPrice
    {
        [Key]
        public int id { set; get; }

        public int productId { set; get; }

        public int customerId { set; get; }

        public double amount { set; get; }

        public string description { set; get; }

        public virtual Product product { set; get; }


    }
}