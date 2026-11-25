namespace ClassifiedAds.Mobile.Core.Notifications;

public sealed record NotificationTarget(Guid Id, string Section)
{
    private static readonly HashSet<string> Sections = ["home", "products", "files", "users", "roles", "configurationentries", "auditlogentries", "notifications"];

    public static NotificationTarget? Parse(string? id, string? section)
    {
        if (!Guid.TryParseExact(id, "D", out var notificationId))
        {
            return null;
        }

        return new NotificationTarget(notificationId, section != null && Sections.Contains(section) ? section : "notifications");
    }
}
