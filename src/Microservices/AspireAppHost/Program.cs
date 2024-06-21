var builder = DistributedApplication.CreateBuilder(args);

var auditLogApi = builder.AddProject<Projects.ClassifiedAds_Services_AuditLog_Api>("ClassifiedAds-Services-AuditLog-Api");
var auditLogGrpc = builder.AddProject<Projects.ClassifiedAds_Services_AuditLog_Grpc>("ClassifiedAds-Services-AuditLog-Grpc");

var configurationApi = builder.AddProject<Projects.ClassifiedAds_Services_Configuration_Api>("ClassifiedAds-Services-Configuration-Api");

var identityApi = builder.AddProject<Projects.ClassifiedAds_Services_Identity_Api>("ClassifiedAds-Services-Identity-Api");
var identityGrpc = builder.AddProject<Projects.ClassifiedAds_Services_Identity_Grpc>("ClassifiedAds-Services-Identity-Grpc");

var notificationApi = builder.AddProject<Projects.ClassifiedAds_Services_Notification_Api>("ClassifiedAds-Services-Notification-Api");
var notificationGrpc = builder.AddProject<Projects.ClassifiedAds_Services_Notification_Grpc>("ClassifiedAds-Services-Notification-Grpc");
var notificationBackground = builder.AddProject<Projects.ClassifiedAds_Services_Notification_Background>("ClassifiedAds-Services-Notification-Background");

var storageApi = builder.AddProject<Projects.ClassifiedAds_Services_Storage_Api>("ClassifiedAds-Services-Storage-Api");

var productApi = builder.AddProject<Projects.ClassifiedAds_Services_Product_Api>("ClassifiedAds-Services-Product-Api");

var graphQlGateway = builder.AddProject<Projects.ClassifiedAds_Gateways_GraphQL>("ClassifiedAds-Gateways-GraphQL");
var apiGateway = builder.AddProject<Projects.ClassifiedAds_Gateways_WebAPI>("ClassifiedAds-Gateways-WebAPI");


var identityServer = builder
    .AddExecutable("ClassifiedAds-IdentityServer", "dotnet", "../../IdentityServer/IdentityServer4/ClassifiedAds.IdentityServer", "run", $"--urls=https://localhost:44367");

builder.Build().Run();