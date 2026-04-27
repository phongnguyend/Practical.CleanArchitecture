namespace ClassifiedAds.WebAPI.ConfigurationOptions;

public class SignalROptions
{
    public bool UseMessagePack { get; set; }

    public BackplaneOptions Backplane { get; set; }
}

public class BackplaneOptions
{
    public string Provider { get; set; }

    public RedisBackplaneOptions Redis { get; set; }

    public AzureSignalRBackplaneOptions Azure { get; set; }
}

public class RedisBackplaneOptions
{
    public string ConnectionString { get; set; }
}

public class AzureSignalRBackplaneOptions
{
    public string ConnectionString { get; set; }
}
