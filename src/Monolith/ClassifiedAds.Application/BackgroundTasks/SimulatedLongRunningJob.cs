using ClassifiedAds.Domain.Notification;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.BackgroundTasks
{
    public class SimulatedLongRunningJob
    {
        private readonly IWebNotification<SendTaskStatusMessage> _notification;

        public SimulatedLongRunningJob(IWebNotification<SendTaskStatusMessage> notification)
        {
            _notification = notification;
        }

        public async Task Run()
        {
            await _notification.SendAsync(new SendTaskStatusMessage { Step = "Step 1", Message = "Begining xxx" });

            Thread.Sleep(2000);

            await _notification.SendAsync(new SendTaskStatusMessage { Step = "Step 1", Message = "Finished xxx" });
        }
    }
}
