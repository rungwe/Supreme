using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Driver
    {
        [Key]
        public int id { set; get; }

        [Required]
        string userid { set; get; }

        [Required]
        public int profileId { set; get; }

        public virtual Profile profile { set; get; }

        public virtual ICollection<DeliveryNote> deliveryNotes { get; set; }

    }
}