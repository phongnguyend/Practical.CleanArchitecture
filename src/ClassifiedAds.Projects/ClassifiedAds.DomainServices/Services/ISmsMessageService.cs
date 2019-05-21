using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices.Services
{
    public interface ISmsMessageService
    {
        void SendSms(string message, string phoneNumber);
    }
}
