namespace ClassifiedAds.Application.FeatureToggles;

public interface IOutboxPublishingToggle
{
    bool IsEnabled();
}
