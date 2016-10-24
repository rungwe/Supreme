using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class BranchProductCreateDTO
    {
        [Required]
        public int productId { set; get; }
        [Required]
        public int customerId { set; get; }
        [Required]
        public double amount { set; get; }

        public string description { get; set; }
        [Required]
        public string sku { set; get; }
    }
}