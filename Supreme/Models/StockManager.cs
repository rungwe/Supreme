using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class StockManager
    {
        [Key]
        public int id { set; get; }

        string userid { set; get; }

        public int profileId { set; get; }

        public virtual Profile profile { set; get; }

        public virtual ICollection<Dispatch> dispaches { get; set; }
    }
}