using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Customer
    {
        [Key]
        public int id { set; get; }

        [Required]
        public string tradingName { set; get; }

        public string reference { set; get; }

        [Required]
        public DateTime registrationDate { set; get; }

        public virtual ICollection<Branch> branches { get; set; }

        public virtual ICollection<Order> orders { get; set; }

        public virtual ICollection<ProductPrice> prices { get; set; }

    }
}