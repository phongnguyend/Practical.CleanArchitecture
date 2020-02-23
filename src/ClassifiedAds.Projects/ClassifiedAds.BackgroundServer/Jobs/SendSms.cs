using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.Jobs
{
    public class SendSms
    {
        private readonly ILogger _logger;

        public SendSms(ILogger<SendEmail> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("Inside Send() method.");
        }
    }
}
