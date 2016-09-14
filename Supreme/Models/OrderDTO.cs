using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class OrderDTO
    {
        
        public int id { get; set; }

        public DateTime date { set; get; }

        public double price { set; get; }

        public string status { set; get; }

        public string orderNumber { set; get; }

        public string invoiceNumber { set; get; }

        public SalesRepDTO salesRep { get; set; }

        public CustomerDTO customer { set; get; }

        public BranchDTO branch { set; get; }

        public string warehouseLocation { set; get; }
        //public ICollection<OrderProduct> orderItems { set; get; }
    }
}