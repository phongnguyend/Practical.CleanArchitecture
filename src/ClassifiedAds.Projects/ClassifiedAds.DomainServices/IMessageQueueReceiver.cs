using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices
{
    public interface IMessageQueueReceiver<T>
    {
        void Receive(Action<T> action);
    }
}
