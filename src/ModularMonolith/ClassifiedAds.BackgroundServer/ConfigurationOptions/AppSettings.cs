using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.MessageBrokers;
using Microsoft.Extensions.Options;

namespace ClassifiedAds.BackgroundServer.ConfigurationOptions;

public class AppSettings
{
    public LoggingOptions Logging { get; set; }

    public string AllowedHosts { get; set; }

    public string CurrentUrl { get; set; }

    public MessageBrokerOptions MessageBroker { get; set; }

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
