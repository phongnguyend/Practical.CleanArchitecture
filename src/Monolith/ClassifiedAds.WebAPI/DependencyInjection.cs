using ClassifiedAds.Infrastructure.Notification.MobilePush;
using ClassifiedAds.WebAPI.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace ClassifiedAds.WebAPI;

public static class DependencyInjection
{
    public static void MapApplicationEndpoints(this WebApplication app)
    {
        MobilePushEndpoints.Map(app);
        ClassifiedAds.WebAPI.Endpoints.NotificationPlaygroundEndpoints.Map(app);
        AuditLogEntriesEndpoints.Map(app);
        ConfigurationEntriesEndpoints.Map(app);
        FilesEndpoints.Map(app);
        ProductsEndpoints.Map(app);
        RolesEndpoints.Map(app);
        UsersEndpoints.Map(app);
    }
}
