using Serilog.Events;

namespace ClassifiedAds.Infrastructure.Logging
{
    public class FileOptions
    {
        public LogEventLevel MinimumLogEventLevel { get; set; }
    }
}
