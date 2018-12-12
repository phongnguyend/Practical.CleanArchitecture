using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices
{
    public interface IMessageQueueSender<T>
    {
        void Send(T message, string exchangeName, string routingKey);
    }
}
