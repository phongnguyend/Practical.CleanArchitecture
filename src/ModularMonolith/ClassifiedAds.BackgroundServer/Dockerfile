FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /ClassifiedAds.ModularMonolith

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.Application/*.csproj ./ClassifiedAds.Application/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/

COPY ./ClassifiedAds.Contracts/*.csproj ./ClassifiedAds.Contracts/
COPY ./ClassifiedAds.Modules.AuditLog/*.csproj ./ClassifiedAds.Modules.AuditLog/
COPY ./ClassifiedAds.Modules.Configuration/*.csproj ./ClassifiedAds.Modules.Configuration/
COPY ./ClassifiedAds.Modules.Identity/*.csproj ./ClassifiedAds.Modules.Identity/
COPY ./ClassifiedAds.Modules.Notification/*.csproj ./ClassifiedAds.Modules.Notification/
COPY ./ClassifiedAds.Modules.Product/*.csproj ./ClassifiedAds.Modules.Product/
COPY ./ClassifiedAds.Modules.Product.EndToEndTests/*.csproj ./ClassifiedAds.Modules.Product.EndToEndTests/
COPY ./ClassifiedAds.Modules.Product.IntegrationTests/*.csproj ./ClassifiedAds.Modules.Product.IntegrationTests/
COPY ./ClassifiedAds.Modules.Product.UnitTests/*.csproj ./ClassifiedAds.Modules.Product.UnitTests/
COPY ./ClassifiedAds.Modules.Storage/*.csproj ./ClassifiedAds.Modules.Storage/

COPY ./ClassifiedAds.BackgroundServer/*.csproj ./ClassifiedAds.BackgroundServer/
COPY ./ClassifiedAds.Migrator/*.csproj ./ClassifiedAds.Migrator/
COPY ./ClassifiedAds.WebAPI/*.csproj ./ClassifiedAds.WebAPI/

COPY ./ClassifiedAds.ModularMonolith.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.BackgroundServer/ClassifiedAds.BackgroundServer.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /ClassifiedAds.ModularMonolith
COPY --from=build-env /ClassifiedAds.ModularMonolith/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.BackgroundServer.dll"]