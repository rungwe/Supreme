using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class RegionDTO
    {
        [Required]
        public string name { set; get; }
    }
}