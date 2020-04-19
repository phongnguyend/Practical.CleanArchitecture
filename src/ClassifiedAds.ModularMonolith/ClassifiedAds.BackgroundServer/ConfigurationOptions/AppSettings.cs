using ClassifiedAds.BackgroundServer.ConfigurationOptions.MessageBroker;
using ClassifiedAds.BackgroundServer.ConfigurationOptions.Storage;
using Microsoft.Extensions.Options;

namespace ClassifiedAds.BackgroundServer.ConfigurationOptions
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public CheckDependency CheckDependency { get; set; }

        public NotificationServer NotificationServer { get; set; }

        public string AllowedHosts { get; set; }

        public string CurrentUrl { get; set; }

        public StorageOptions Storage { get; set; }

        public MessageBrokerOptions MessageBroker { get; set; }

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
}
