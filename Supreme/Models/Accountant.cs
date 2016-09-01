using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Accountant
    {
        [Key]
        public int id { set; get; }

        [Required]
        public string user_id { set; get; }

        [Required]
        public int profileId { set; get; }

        public Profile profile { set; get; }

        public virtual ICollection<Invoice> invoices { get; set; }
    }
}