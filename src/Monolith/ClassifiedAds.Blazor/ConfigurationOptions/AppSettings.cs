using Microsoft.Extensions.Options;

namespace ClassifiedAds.Blazor.ConfigurationOptions
{
    public class AppSettings
    {
        public OpenIdConnect OpenIdConnect { get; set; }

        public ResourceServer ResourceServer { get; set; }
       
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
}
