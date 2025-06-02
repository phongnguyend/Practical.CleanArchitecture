using ClassifiedAds.Infrastructure.Caching;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.Messaging;
using ClassifiedAds.Infrastructure.Monitoring;
using Microsoft.Extensions.Options;

namespace ClassifiedAds.Background.ConfigurationOptions;

public class AppSettings
{
    public LoggingOptions Logging { get; set; }

    public CachingOptions Caching { get; set; }

    public MonitoringOptions Monitoring { get; set; }

    public MessagingOptions Messaging { get; set; }

    public ModulesOptions Modules { get; set; }

    public ValidateOptionsResult Validate()
    {
        return ValidateOptionsResult.Success;
    }
}

public class AppSettingsValidation : IValidateOptions<AppSettings>
{
    public ValidateOptionsResult Validate(string name, AppSettings options)
    {
        return options.Validate();
    }
}
