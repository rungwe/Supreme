using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class BranchCreateDTO
    {

        [Required]
        public int customerId { set; get; }

        [Required]
        public string name { set; get; }

        public string address { set; get; }

        public string telephone { set; get; }

        public string email { set; get; }

        [Required]
        public int regionId { set; get; }

        public int salesRepId { set; get; }

    }
}