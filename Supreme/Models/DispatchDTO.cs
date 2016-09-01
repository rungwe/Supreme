using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class DispatchDTO
    {
        
        public int id { set; get; }

        public int orderId { set; get; }
   
        public StockManagerDTO stockManager { set; get; }

      
    }
}