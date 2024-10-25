var builder = DistributedApplication.CreateBuilder(args);

var migrator = builder.AddProject<Projects.ClassifiedAds_Migrator>("ClassifiedAds-Migrator");
var background = builder.AddProject<Projects.ClassifiedAds_Background>("ClassifiedAds-Background");
var webApi = builder.AddProject<Projects.ClassifiedAds_WebAPI>("ClassifiedAds-WebAPI");

var identityServer = builder
    .AddExecutable("ClassifiedAds-IdentityServer", "dotnet", "../../IdentityServer/IdentityServer4/ClassifiedAds.IdentityServer", "run", $"--urls=https://localhost:44367");

builder.Build().Run();