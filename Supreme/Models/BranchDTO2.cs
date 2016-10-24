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

        public string location { set; get; }

        public string telephone { set; get; }

        public string telephone2 { set; get; }

        public string branchManager { set; get; }

        public int monthlyBudget { get; set; }

        public string email { set; get; }

        public int regionId { set; get; }

        public string tradingName { set; get; }

        public MerchantDTO merchant { set; get; }

        public SalesRepDTO salesRep { set; get; }

        public CustomerDTO2 customer { set; get; }

        public Bank bank { set; get; }
    }
}