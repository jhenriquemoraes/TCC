using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.DTOs.Requests
{
    public class CreateOrderRequest
    {
        public CustomerRequest Customer { get; set; }
        public List<ProductRequest> Products { get; set; }
        public PaymentRequest Payment { get; set; }
        public DeliveryRequest Delivery { get; set; }
    }
}