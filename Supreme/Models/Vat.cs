using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class Vat
    {
        [Key]
        public int id { set; get; }

        public DateTime date { set; get; }

        public virtual Invoice invoice { set; get; }

        public double initialVat { set; get; }

        public double remainingVat { set; get; }

        public virtual Branch branch { set; get; }

    }
}