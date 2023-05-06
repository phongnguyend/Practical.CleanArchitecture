namespace ClassifiedAds.WebMVC.ConfigurationOptions;

public class CookiePolicyOptions: Microsoft.AspNetCore.Builder.CookiePolicyOptions
{
    public bool IsEnabled { get; set; }
}
