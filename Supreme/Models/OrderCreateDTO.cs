using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class OrderCreateDTO
    {

        [Required]
        public int branchId { set; get; }

        [Required]
        public ICollection<OrderItem> orderItem {set; get;}
    }
}