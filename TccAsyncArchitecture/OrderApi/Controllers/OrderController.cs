using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderApi.DTOs.Requests;
using OrderApi.Messaging;
using MessageContracts;
using MessageContracts.Events;
using MessageContracts.DTOs;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMessagePublisher _messagePublisher;
        public OrderController(IMessagePublisher messagePublisher)
        {   
           this._messagePublisher = messagePublisher;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var orderId = Guid.NewGuid();

            var orderCreated = new OrderCreated
            {
                OrderId = orderId,
                Customer = new CustomerData
                {
                    CustomerId = request.Customer.CustomerId,
                    Name = request.Customer.Name,
                },
                Products = request.Products.Select(p => new ProductData
                {
                    ProductId = p.ProductId,
                    SaleQuantity = p.SaleQuantity,
                    SaleValue = p.SaleValue
                }).ToList(),
                Payment = new PaymentData
                {
                    PaymentMethod = request.Payment.Method,
                },
                Delivery = new DeliveryData
                {
                    Address = request.Delivery.Address,
                }
            };

            await _messagePublisher.PublishAsync(orderCreated);

            return Ok(new
            {
                orderCreated.OrderId,
                message = "Order created successfully",
            }
            );
        }
    }
}