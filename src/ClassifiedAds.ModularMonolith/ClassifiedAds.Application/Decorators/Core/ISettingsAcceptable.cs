namespace ClassifiedAds.Application.Decorators.Core
{
    internal interface ISettingsAcceptable
    {
        void Accept(ISettingsProvider settingsProvider);
    }
}