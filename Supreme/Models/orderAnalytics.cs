using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class orderAnalytics
    {
        public DateTime date { set; get; }

        public double price { set; get; }

        public string status { set; get; }

        public string warehouseLocation { set; get; }
    }
}