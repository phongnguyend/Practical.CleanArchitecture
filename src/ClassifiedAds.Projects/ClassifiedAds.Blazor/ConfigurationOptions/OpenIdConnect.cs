using Microsoft.Extensions.Options;
using System;

namespace ClassifiedAds.Blazor.ConfigurationOptions
{
    public class OpenIdConnect
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public ValidateOptionsResult Validate()
        {
            if (!Uri.TryCreate(Authority, UriKind.Absolute, out _))
            {
                return ValidateOptionsResult.Fail($"{Authority} is not a valid URI.");
            }

            return ValidateOptionsResult.Success;
        }
    }

    public class OpenIdConnectValidation : IValidateOptions<OpenIdConnect>
    {
        public ValidateOptionsResult Validate(string name, OpenIdConnect options)
        {
            return options.Validate();
        }
    }
}
