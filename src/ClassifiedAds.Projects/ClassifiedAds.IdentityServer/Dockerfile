FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /ClassifiedAds.Projects

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.ApplicationServices/*.csproj ./ClassifiedAds.ApplicationServices/
COPY ./ClassifiedAds.BackgroundServices/*.csproj ./ClassifiedAds.BackgroundServices/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.DomainServices/*.csproj ./ClassifiedAds.DomainServices/
COPY ./ClassifiedAds.DomainServices.UnitTests/*.csproj ./ClassifiedAds.DomainServices.UnitTests/
COPY ./ClassifiedAds.GraphQL/*.csproj ./ClassifiedAds.GraphQL/
COPY ./ClassifiedAds.GraphQL.IntegrationTests/*.csproj ./ClassifiedAds.GraphQL.IntegrationTests/
COPY ./ClassifiedAds.GRPC/*.csproj ./ClassifiedAds.GRPC/
COPY ./ClassifiedAds.GRPC.IntegrationTests/*.csproj ./ClassifiedAds.GRPC.IntegrationTests/
COPY ./ClassifiedAds.IdentityServer/*.csproj ./ClassifiedAds.IdentityServer/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/
COPY ./ClassifiedAds.Migrator/*.csproj ./ClassifiedAds.Migrator/
COPY ./ClassifiedAds.NotificationServer/*.csproj ./ClassifiedAds.NotificationServer/
COPY ./ClassifiedAds.Ocelot/*.csproj ./ClassifiedAds.Ocelot/
COPY ./ClassifiedAds.Ocelot.IntegrationTests/*.csproj ./ClassifiedAds.Ocelot.IntegrationTests/
COPY ./ClassifiedAds.Persistence/*.csproj ./ClassifiedAds.Persistence/
COPY ./ClassifiedAds.WebAPI/*.csproj ./ClassifiedAds.WebAPI/
COPY ./ClassifiedAds.WebAPI.IntegrationTests/*.csproj ./ClassifiedAds.WebAPI.IntegrationTests/
COPY ./ClassifiedAds.WebMVC/*.csproj ./ClassifiedAds.WebMVC/
COPY ./ClassifiedAds.WebMVC.AutomationTests/*.csproj ./ClassifiedAds.WebMVC.AutomationTests/
COPY ./ClassifiedAds.Projects.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.IdentityServer/ClassifiedAds.IdentityServer.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /ClassifiedAds.Projects
COPY --from=build-env /ClassifiedAds.Projects/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.IdentityServer.dll"]