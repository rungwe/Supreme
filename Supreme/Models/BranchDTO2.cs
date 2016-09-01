using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class BranchDTO2
    {
        public int id { set; get; }

        public string name { set; get; }

        public string address { set; get; }

        public string telephone { set; get; }

        public string email { set; get; }

        public int regionId { set; get; }

        public string tradingName { set; get; }

        public CustomerDTO2 customer { set; get; }
    }
}