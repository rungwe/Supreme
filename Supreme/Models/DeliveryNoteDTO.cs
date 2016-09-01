using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supreme.Models
{
    public class DeliveryNoteDTO
    {
        public int id { set; get; }

        public DateTime date { set; get; }

        public int orderId { set; get; }
        
        public int driverId { set; get; }

        public string status { set; get; }
    }
}