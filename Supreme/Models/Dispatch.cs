using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Dispatch
    {
        [Key]
        public int id { set; get; }

        [Required]
        public int orderId { set; get; }

        public virtual Order order { set; get; }

        public DateTime date { set; get; }

        [Required]
        public int stockManagerId { set; get; }

        public virtual StockManager stockManager { set; get; }

        public string status { set; get; }

        
       
    }
}