namespace ClassifiedAds.BlazorServerSide.ConfigurationOptions;

public class Azure
{
    public SignalR SignalR { get; set; }
}

public class SignalR
{
    public bool IsEnabled { get; set; }
}
