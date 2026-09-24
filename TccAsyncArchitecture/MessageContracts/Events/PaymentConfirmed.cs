using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageContracts.DTOs;

namespace MessageContracts.Events
{
    public class PaymentConfirmed
    {
        public Guid OrderId { get; set; }
        public List<ProductData> Products { get; set; }
    }
}