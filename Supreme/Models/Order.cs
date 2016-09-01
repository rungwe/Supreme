using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Order
    {
        [Key]
        public int id { get; set; }

        public DateTime date { set; get; }

        public int salesRepId { set; get; }

        public virtual SalesRep salesRep { get; set; }

        public int customerId { set; get; }

        public string status { set; get; }

        public int branchId { set; get; }

        public virtual Branch branch { set; get; }

        [Required]
        public double price { set; get; }

        public virtual ICollection<OrderProduct> orderProducts { set; get; }




    }
}