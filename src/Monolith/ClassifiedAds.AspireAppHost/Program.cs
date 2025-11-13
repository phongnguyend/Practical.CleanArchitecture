var builder = DistributedApplication.CreateBuilder(args);

var migrator = builder.AddProject<Projects.ClassifiedAds_Migrator>("ClassifiedAds-Migrator");
var background = builder.AddProject<Projects.ClassifiedAds_Background>("ClassifiedAds-Background");
var webApi = builder.AddProject<Projects.ClassifiedAds_WebAPI>("ClassifiedAds-WebAPI").WithHttpHealthCheck("/healthz");
var webMvc = builder.AddProject<Projects.ClassifiedAds_WebMVC>("ClassifiedAds-WebMVC").WithHttpHealthCheck("/healthz");
var blazorServerSide = builder.AddProject<Projects.ClassifiedAds_BlazorServerSide>("ClassifiedAds-BlazorServerSide").WithHttpHealthCheck("/healthz");
var blazorWebAssembly = builder.AddProject<Projects.ClassifiedAds_BlazorWebAssembly>("ClassifiedAds-BlazorWebAssembly");

var identityServer = builder
    .AddExecutable("IdentityServer", "dotnet", "../../IdentityServers/OpenIddict/ClassifiedAds.IdentityServer", "run", $"--urls=https://localhost:44367");

var identityServerMigrator = builder
    .AddExecutable("IdentityServer-Migrator", "dotnet", "../../IdentityServers/OpenIddict/ClassifiedAds.Migrator", "run");

var webhook = builder.AddExternalService("Webhook", "https://ddddotnet-webhook-server.azurewebsites.net").WithHttpHealthCheck("");

builder.Build().Run();