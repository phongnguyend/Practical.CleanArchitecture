using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Infrastructure
{
    public interface IMessageQueueReceiver<T>
    {
        void Receive(Action<T> action);
    }
}
