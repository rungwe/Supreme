using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class ProductCreateDTO
    {
        [Required]
        public string name { set; get; }
        [Required]
        public string sku { set; get; }
        [Required]
        public string description { set; get; }

        [Required]
        public string type { set; get; }
    }
}