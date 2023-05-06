using ClassifiedAds.Infrastructure.Caching;
using ClassifiedAds.Infrastructure.Configuration;
using ClassifiedAds.Infrastructure.Interceptors;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Infrastructure.Storages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ClassifiedAds.WebMVC.ConfigurationOptions;

public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; }

    public CheckDependency CheckDependency { get; set; }

    public LoggingOptions Logging { get; set; }

    public CachingOptions Caching { get; set; }

    public MonitoringOptions Monitoring { get; set; }

    public OpenIdConnect OpenIdConnect { get; set; }

    public ResourceServer ResourceServer { get; set; }

    public string AllowedHosts { get; set; }

    public string CurrentUrl { get; set; }

    public ConfigurationProviders ConfigurationProviders { get; set; }

    public StorageOptions Storage { get; set; }

    public MessageBrokerOptions MessageBroker { get; set; }

    public CookiePolicyOptions CookiePolicyOptions { get; set; }

    public Dictionary<string, string> SecurityHeaders { get; set; }

    public InterceptorsOptions Interceptors { get; set; }

    public Azure Azure { get; set; }

    public ValidateOptionsResult Validate()
    {
        var validationRs = OpenIdConnect.Validate();

        if (validationRs.Failed)
        {
            return validationRs;
        }

        validationRs = ResourceServer.Validate();

        if (validationRs.Failed)
        {
            return validationRs;
        }

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
