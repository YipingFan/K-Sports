using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoDelivery.Models
{
    public class Order
    {
        //public DateTime Date { get; set; }
        public string Item { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
    }
}