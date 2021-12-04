using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task01_Module01.Models
{
    public class OrderDesc
    {
        public int  OrderId { get; set; }
        public string ShippingArea { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryDays { get; set; }
        public double DeliveryCharges { get; set; }
    }
}
