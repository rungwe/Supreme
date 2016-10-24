using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class SalesLedger
    {
        [Key]
        public int id { get; set; }

        public DateTime date { get; set; }
        [Required]
        public virtual DeliveryNote deliveryNote { get; set; }

        [Required]
        public virtual Order order { get; set; }

        


    }
}