using ClassifiedAds.BlazorServerSide.ConfigurationOptions;
using Microsoft.Extensions.DependencyInjection;

namespace ClassifiedAds.BlazorServerSide.Configurations;

public static class SignalRConfiguration
{
    public static IServiceCollection AddClassifiedAdsSignalR(this IServiceCollection services, AppSettings appSettings)
    {
        var signalRBuilder = services.AddSignalR();

        if (appSettings.SignalR?.UseMessagePack == true)
        {
            signalRBuilder.AddMessagePackProtocol();
        }

        if (appSettings.SignalR?.Backplane?.Provider == "Redis")
        {
            signalRBuilder.AddStackExchangeRedis(appSettings.SignalR.Backplane.Redis.ConnectionString);
        }
        else if (appSettings.SignalR?.Backplane?.Provider == "Azure")
        {
            signalRBuilder.AddAzureSignalR(appSettings.SignalR.Backplane.Azure.ConnectionString);
        }

        return services;
    }
}
