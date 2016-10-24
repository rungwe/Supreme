using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Bank
    {
        [Key]
        public int id { set; get; }
        [Required]
        public string name { set; get; }
        [Required]
        public string branch_name { set; get; }
        public string branch_code { set; get; }
        [Required]
        public string account_holder { set; get; }
        [Required]
        public string account_number { set; get; }
    }
}