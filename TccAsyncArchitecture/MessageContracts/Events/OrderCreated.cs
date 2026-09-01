using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageContracts.DTOs;

namespace MessageContracts.Events
{
    public class OrderCreated
    {
        public Guid OrderId { get; set; }
        public CustomerData Customer { get; set; }
        public List<ProductData> Products { get; set; }
        public PaymentData Payment { get; set; }
        public DeliveryData Delivery { get; set; }
    }
}