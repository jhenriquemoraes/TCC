using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.DTOs.Requests
{
    public class ProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}