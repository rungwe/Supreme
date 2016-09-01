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

        [Required]
        public string address { set; get; }

        public string telephone { set; get; }

        [EmailAddress]
        public string email { set; get; }

        public int regionId { set; get; }

        public int salesRepId { set; get; }

    }
}