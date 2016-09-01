using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Profile
    {
        [Key]
        public int id { set; get; }

        [Required]
        public string userid { set; get; }

        public string firstname { set; get; }

        public string lastname { set; get; }

        public string middlename { set; get; }

        [Required]
        [EmailAddress]
        public string email { set; get; }


        public string phone { set; get; }

        [Required]
        public string position { set; get; }

        public string address { set; get; }

        public string town { set; get; }

        public string status { set; get; }

        public string profile_pic { set; get; }

        public DateTime registrationDate { get; set; }
    }
}