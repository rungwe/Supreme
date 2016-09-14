using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class InvoiceCreateDTO
    {
        
        [Required]
        public int orderid { set; get; }
       
       
        public string invoiceNumber { set; get; }

        [Required]
        public string warehouseLocation { set; get; }
    }
}