var builder = DistributedApplication.CreateBuilder(args);

var migrator = builder.AddProject<Projects.ClassifiedAds_Migrator>("ClassifiedAds-Migrator");
var background = builder.AddProject<Projects.ClassifiedAds_Background>("ClassifiedAds-Background");
var graphQL = builder.AddProject<Projects.ClassifiedAds_GraphQL>("ClassifiedAds-GraphQL").WithHttpsHealthCheck("/healthz");
var webApi = builder.AddProject<Projects.ClassifiedAds_WebAPI>("ClassifiedAds-WebAPI").WithHttpsHealthCheck("/healthz");
var webMvc = builder.AddProject<Projects.ClassifiedAds_WebMVC>("ClassifiedAds-WebMVC").WithHttpsHealthCheck("/healthz");
var blazorServerSide = builder.AddProject<Projects.ClassifiedAds_BlazorServerSide>("ClassifiedAds-BlazorServerSide").WithHttpsHealthCheck("/healthz");
var blazorWebAssembly = builder.AddProject<Projects.ClassifiedAds_BlazorWebAssembly>("ClassifiedAds-BlazorWebAssembly");

var identityServer = builder
    .AddExecutable("ClassifiedAds-IdentityServer", "dotnet", "../../IdentityServer/IdentityServer4/ClassifiedAds.IdentityServer", "run", $"--urls=https://localhost:44367");

builder.Build().Run();