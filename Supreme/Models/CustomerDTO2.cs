using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class CustomerDTO2
    {
        public int id { set; get; }

        public string tradingName { set; get; }

        public string reference { set; get; }

        public DateTime registrationDate { set; get; }
    }
}