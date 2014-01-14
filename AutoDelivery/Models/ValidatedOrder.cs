using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoDelivery.Repository;
using GeoCoding.Google;

namespace AutoDelivery.Models
{
    public class ValidatedOrder
    {
        public Order Order { get; set; }
        public IList<GoogleAddress> GoogleAddresses { get; set; }
        public Region Region { get; set; }
    }
}