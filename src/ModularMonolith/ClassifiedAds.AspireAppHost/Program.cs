var builder = DistributedApplication.CreateBuilder(args);

var migrator = builder.AddProject<Projects.ClassifiedAds_Migrator>("ClassifiedAds-Migrator");
var background = builder.AddProject<Projects.ClassifiedAds_Background>("ClassifiedAds-Background");
var webApi = builder.AddProject<Projects.ClassifiedAds_WebAPI>("ClassifiedAds-WebAPI");

var identityServer = builder
    .AddExecutable("IdentityServer", "dotnet", "../../IdentityServers/OpenIddict/ClassifiedAds.IdentityServer", "run", $"--urls=https://localhost:44367");

var identityServerMigrator = builder
    .AddExecutable("IdentityServer-Migrator", "dotnet", "../../IdentityServers/OpenIddict/ClassifiedAds.Migrator", "run");

var webhook = builder.AddExternalService("Webhook", "https://ddddotnet-webhook-server.azurewebsites.net").WithHttpHealthCheck("");

builder.Build().Run();