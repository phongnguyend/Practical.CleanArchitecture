using System.Collections.Generic;

namespace ClassifiedAds.Gateways.WebAPI.ConfigurationOptions;

public class OcelotOptions
{
    public string DefaultDownstreamScheme { get; set; }

    public OcelotRoutesOptions Routes { get; set; }
}

public class OcelotRoutesOptions : Dictionary<string, OcelotRouteOptions>
{

}

public class OcelotRouteOptions
{
    public List<string> UpstreamPathTemplates { get; set; }

    public string Downstream { get; set; }
}
