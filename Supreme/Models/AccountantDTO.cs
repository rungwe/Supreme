using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class AccountantDTO
    {
        public int Id { set; get; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string middlename { set; get; }
    }
}