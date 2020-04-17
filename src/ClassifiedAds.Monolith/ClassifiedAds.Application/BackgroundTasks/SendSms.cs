using Microsoft.Extensions.Logging;

namespace ClassifiedAds.Application.BackgroundTasks
{
    public class SendSms
    {
        private readonly ILogger _logger;

        public SendSms(ILogger<SendSms> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("Inside Send() method.");
        }
    }
}
