using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Invoice
    {
        [Key]
        public int id { set; get; }

        public DateTime date { set; get; }

        public int orderid { set; get; }

        public virtual Order order { set; get; }

        public int accountant_id { set; get; }

        public virtual Accountant account { set; get; }

        public string status { set; get; }

        public string invoiceNumber { set; get; }



    }
}