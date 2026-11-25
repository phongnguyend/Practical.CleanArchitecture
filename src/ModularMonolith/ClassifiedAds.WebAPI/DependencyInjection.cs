using ClassifiedAds.Infrastructure.Notification.MobilePush;
using ClassifiedAds.Modules.AuditLog.Endpoints;
using ClassifiedAds.Modules.Configuration.Endpoints;
using ClassifiedAds.Modules.Identity.Endpoints;
using ClassifiedAds.Modules.Product.Endpoints;
using ClassifiedAds.Modules.Storage.Endpoints;
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
        RolesEndpoints.Map(app);
        UsersEndpoints.Map(app);
        ProductsEndpoints.Map(app);
        FilesEndpoints.Map(app);
    }
}
