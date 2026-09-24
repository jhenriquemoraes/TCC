using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message);
    }
}