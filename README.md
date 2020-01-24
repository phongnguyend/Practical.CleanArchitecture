[Clean Architecture: Patterns, Practices, and Principles | Pluralsight](https://app.pluralsight.com/library/courses/clean-architecture-patterns-practices-principles/table-of-contents)

# Database Centric vs Domain Centric Architecture
![alt text](/docs/imgs/database-centrics-vs-domain-centric-architecture.png)

# Hexagonal Architecture
![alt text](/docs/imgs/hexagonal-architecture.png)

# Onion Architecture
![alt text](/docs/imgs/onion-architecture.png)

# The Clean Architecture
![alt text](/docs/imgs/the-clean-architecture.png)


# Classic Three-layer Architecture
![alt text](/docs/imgs/Classic-Three-layer-Architecture.png)


# Modern Four-layer Architecture
![alt text](/docs/imgs/Modern-Four-layer-Architecture.png)


# Layer Dependencies
![alt text](/docs/imgs/Layer-Dependencies.png)

# Layer Examples
![alt text](/docs/imgs/Layer-Examples.png)

# Solution Structure
![alt text](/docs/imgs/code-solution-structure.png)

# How to Run:
- Update Connection Strings:

| Project  | Configuration File | Configuration Key |
| -------- | ------------------ | ----------------- |
| ClassifiedAds.Migrator | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.Migrator/appsettings.json) | ConnectionStrings:ClassifiedAds |
| ClassifiedAds.BackgroundServices | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.BackgroundServices/appsettings.json) | ConnectionStrings:ClassifiedAds |
| ClassifiedAds.GRPC | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.GRPC/appsettings.json) | ConnectionStrings:ClassifiedAds |
| ClassifiedAds.IdentityServer | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.IdentityServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
| ClassifiedAds.NotificationServer | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.NotificationServer/appsettings.json) | |
| ClassifiedAds.WebAPI | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebAPI/appsettings.json) | ConnectionStrings:ClassifiedAds |
| ClassifiedAds.WebMVC | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebMVC/appsettings.json) | ConnectionStrings:ClassifiedAds |
| ClassifiedAds.GraphQL | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.GraphQL/appsettings.json) | ConnectionStrings:ClassifiedAds |
| ClassifiedAds.Ocelot | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.Ocelot/appsettings.json) |  |

- Install **dotnet-ef** cli:
```
dotnet tool install --global dotnet-ef --version="3.1"
```
- Navigate to [ClassifiedAds.Migrator](/src/ClassifiedAds.Projects/ClassifiedAds.Migrator/) and run these commands:
```
dotnet ef migrations add Init --context AdsDbContext
dotnet ef database update --context AdsDbContext
dotnet ef database update --context ConfigurationDbContext
dotnet ef database update --context PersistedGrantDbContext
```
