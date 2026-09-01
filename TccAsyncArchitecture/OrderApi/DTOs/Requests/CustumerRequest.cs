using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.DTOs.Requests
{
    public class CustomerRequest
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }
}