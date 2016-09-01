using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class CustomerDTO
    {
        public int id { set; get; }

        public string tradingName { set; get; }

        public DateTime registrationDate { set; get; }

        public virtual ICollection<BranchDTO> branches { get; set; }

        public virtual ICollection<OrderDTO> orders { get; set; }

        public virtual ICollection<OrderDTO> prices { get; set; }
    }
}