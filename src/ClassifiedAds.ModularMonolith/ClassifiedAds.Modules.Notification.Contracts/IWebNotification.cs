using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.Contracts
{
    public interface IWebNotification
    {
        void Send<T>(string endpoint, string eventName, T message);
        Task SendAsync<T>(string endpoint, string eventName, T message);
    }
}
