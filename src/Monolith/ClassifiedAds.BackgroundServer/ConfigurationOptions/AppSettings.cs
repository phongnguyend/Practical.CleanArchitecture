using ClassifiedAds.Infrastructure.IdentityProviders.Auth0;
using ClassifiedAds.Infrastructure.IdentityProviders.Azure;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Notification;
using ClassifiedAds.Infrastructure.Storages;
using Microsoft.Extensions.Options;

namespace ClassifiedAds.BackgroundServer.ConfigurationOptions;

public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; }

    public LoggingOptions Logging { get; set; }

    public string AllowedHosts { get; set; }

    public string CurrentUrl { get; set; }

    public StorageOptions Storage { get; set; }

    public MessageBrokerOptions MessageBroker { get; set; }

    public NotificationOptions Notification { get; set; }

    public IdentityProvidersOptions IdentityProviders { get; set; }

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

public class IdentityProvidersOptions
{
    public Auth0Options Auth0 { get; set; }

    public AzureAdB2COptions AzureActiveDirectoryB2C { get; set; }
}

