using Microsoft.Extensions.Options;
using System;

namespace ClassifiedAds.WebMVC.ConfigurationOptions
{
    public class ResourceServer
    {
        public string Endpoint { get; set; }

        public string PublicEndpoint { get; set; }

        public ValidateOptionsResult Validate()
        {
            if (!Uri.TryCreate(Endpoint, UriKind.Absolute, out _))
            {
                return ValidateOptionsResult.Fail($"{Endpoint} is not a valid URI.");
            }

            return ValidateOptionsResult.Success;
        }
    }

    public class ResourceServerValidation : IValidateOptions<ResourceServer>
    {
        public ValidateOptionsResult Validate(string name, ResourceServer options)
        {
            return options.Validate();
        }
    }
}
