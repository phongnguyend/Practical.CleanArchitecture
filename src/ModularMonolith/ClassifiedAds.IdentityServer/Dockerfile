FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /ClassifiedAds.ModularMonolith

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.Application/*.csproj ./ClassifiedAds.Application/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/

COPY ./ClassifiedAds.Modules.AuditLog/*.csproj ./ClassifiedAds.Modules.AuditLog/
COPY ./ClassifiedAds.Modules.AuditLog.Contracts/*.csproj ./ClassifiedAds.Modules.AuditLog.Contracts/
COPY ./ClassifiedAds.Modules.Auth/*.csproj ./ClassifiedAds.Modules.Auth/
COPY ./ClassifiedAds.Modules.Configuration/*.csproj ./ClassifiedAds.Modules.Configuration/
COPY ./ClassifiedAds.Modules.Identity/*.csproj ./ClassifiedAds.Modules.Identity/
COPY ./ClassifiedAds.Modules.Identity.Contracts/*.csproj ./ClassifiedAds.Modules.Identity.Contracts/
COPY ./ClassifiedAds.Modules.Notification/*.csproj ./ClassifiedAds.Modules.Notification/
COPY ./ClassifiedAds.Modules.Notification.Contracts/*.csproj ./ClassifiedAds.Modules.Notification.Contracts/
COPY ./ClassifiedAds.Modules.Product/*.csproj ./ClassifiedAds.Modules.Product/
COPY ./ClassifiedAds.Modules.Product.EndToEndTests/*.csproj ./ClassifiedAds.Modules.Product.EndToEndTests/
COPY ./ClassifiedAds.Modules.Product.IntegrationTests/*.csproj ./ClassifiedAds.Modules.Product.IntegrationTests/
COPY ./ClassifiedAds.Modules.Product.UnitTests/*.csproj ./ClassifiedAds.Modules.Product.UnitTests/
COPY ./ClassifiedAds.Modules.Storage/*.csproj ./ClassifiedAds.Modules.Storage/

COPY ./ClassifiedAds.BackgroundServer/*.csproj ./ClassifiedAds.BackgroundServer/
COPY ./ClassifiedAds.IdentityServer/*.csproj ./ClassifiedAds.IdentityServer/
COPY ./ClassifiedAds.Migrator/*.csproj ./ClassifiedAds.Migrator/
COPY ./ClassifiedAds.WebAPI/*.csproj ./ClassifiedAds.WebAPI/

COPY ./ClassifiedAds.ModularMonolith.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.IdentityServer/ClassifiedAds.IdentityServer.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /ClassifiedAds.ModularMonolith
COPY --from=build-env /ClassifiedAds.ModularMonolith/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.IdentityServer.dll"]