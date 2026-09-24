using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageContracts.Events
{
    public class ProductItemSeparated
    {
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public int SaleQuantity { get; set; }
    }
}