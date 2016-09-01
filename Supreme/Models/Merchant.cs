using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Merchant
    {
        [Key]
        public int id { set; get; }

        string user_id { set; get; }

        public int profileId { set; get; }

        public virtual Profile profile { set; get; }

        //public virtual ICollection<SalesRep> salesReps { get; set; }
    }
}