using ClassifiedAds.Application.FeatureToggles;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ClassifiedAds.Infrastructure.FeatureToggles.OutboxPublishingToggle;

public class FileBasedOutboxPublishingToggle : IOutboxPublishingToggle
{
    private readonly IConfiguration _configuration;

    public FileBasedOutboxPublishingToggle(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsEnabled()
    {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Practical.CleanArchitecture", "FeatureToggles", "outbox_publishing_disabled.txt");

        return !File.Exists(filePath);
    }
}
