using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class InvoiceDTO
    {
        
        public int id { set; get; }

        public DateTime date { set; get; }

        public int orderid { set; get; }

        public OrderDTO order { set; get; }

        public  AccountantDTO accountant { set; get; }

        public string status { set; get; }

        public string invoiceNumber { set; get; }
    }
}