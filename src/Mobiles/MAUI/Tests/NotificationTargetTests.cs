using ClassifiedAds.Mobile.Core.Notifications;
using Xunit;

namespace ClassifiedAds.Mobile.Tests;

public sealed class NotificationTargetTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("../../../users")]
    public void InvalidNotificationIdsCannotNavigate(string? id)
    {
        Assert.Null(NotificationTarget.Parse(id, "products"));
    }

    [Theory]
    [InlineData("products", "products")]
    [InlineData("auditlogentries", "auditlogentries")]
    [InlineData("https://example.com", "notifications")]
    [InlineData("../users", "notifications")]
    [InlineData(null, "notifications")]
    public void OnlyKnownSectionsAreAccepted(string? section, string expected)
    {
        Assert.Equal(expected, NotificationTarget.Parse("11111111-1111-4111-8111-111111111111", section)!.Section);
    }
}
