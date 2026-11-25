namespace ClassifiedAds.Infrastructure.Notification.MobilePush;

public sealed class MobilePushOptions
{
    public string Provider { get; set; } = "Fake";

    public FirebaseMobilePushOptions Firebase { get; set; } = new();

    public bool AllowTestMessages { get; set; }

    public bool UsedFake()
    {
        return Provider == "Fake";
    }

    public bool UsedFirebase()
    {
        return Provider == "Firebase";
    }
}

public sealed class FirebaseMobilePushOptions
{
    public string ProjectId { get; set; } = "";
}
