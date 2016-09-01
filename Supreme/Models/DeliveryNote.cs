using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class DeliveryNote
    {
        [Key]
        public int id { set; get; }

        public DateTime date { set; get; }

        [Required]
        public int orderId { set; get; }
        [Required]
        public int driverId { set; get; }

        public string status { set; get; }

        public virtual Driver driver { set; get; }

        //public virtual Order order { set; get; }

    }
}