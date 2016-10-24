using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Branch
    {
        [Key]
        public int id { set; get; }

        [Required]
        public int customerId { set; get; }

        public virtual Customer customer { set; get; }

        [Required]
        public string name { set; get; }    

        public string reference { set; get; }

        public string address { set; get; }

        public string location { set; get; }

        public string telephone { set; get; }

        public string telephone2 { set; get; }

        public string branchManager { set; get; }

        public int monthlyBudget { get; set; }

        [EmailAddress]
        public string email { set; get; }

        public int regionId { set; get; }

        public int salesRepId { set; get; }

        public virtual SalesRep salesRep { get; set; }

        public int merchantId { set; get; }

        public virtual Merchant merchant { get; set; }

        public virtual Bank bank { get; set; }

    }
}