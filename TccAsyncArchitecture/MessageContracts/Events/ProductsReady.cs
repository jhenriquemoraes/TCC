using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageContracts.Events
{
    public class ProductsReady
    {
        public Guid OrderId { get; set; }
    }
}