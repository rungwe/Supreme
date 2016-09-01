using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class SalesRep
    {
        [Key]
        public int id { set; get; }

        public string userid { set; get; }

        public int profileId { set; get; }

        public virtual Profile profile {set;get;}

        public virtual ICollection<Order> orders { get; set; }

    }
}