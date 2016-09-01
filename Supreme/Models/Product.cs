using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Product
    {
        [Key]
        public int id { set; get; }
        
        public string name { set; get; }

        public string sku { set; get; }

        public string description { set; get; }

        public string type { set; get; }
    }
}