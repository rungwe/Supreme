using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class CustomerCreateDTO
    {
        [Required]
        public string tradingName { set; get; }

        //public string reference { set; get; }


    }
}