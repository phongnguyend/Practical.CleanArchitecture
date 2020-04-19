FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /ClassifiedAds.Projects

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.Application/*.csproj ./ClassifiedAds.Application/
COPY ./ClassifiedAds.BackgroundServer/*.csproj ./ClassifiedAds.BackgroundServer/
COPY ./ClassifiedAds.Blazor/*.csproj ./ClassifiedAds.Blazor/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.EndToEndTests/*.csproj ./ClassifiedAds.EndToEndTests/
COPY ./ClassifiedAds.GraphQL/*.csproj ./ClassifiedAds.GraphQL/
COPY ./ClassifiedAds.IdentityServer/*.csproj ./ClassifiedAds.IdentityServer/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/
COPY ./ClassifiedAds.IntegrationTests/*.csproj ./ClassifiedAds.IntegrationTests/
COPY ./ClassifiedAds.Migrator/*.csproj ./ClassifiedAds.Migrator/
COPY ./ClassifiedAds.NotificationServer/*.csproj ./ClassifiedAds.NotificationServer/
COPY ./ClassifiedAds.Ocelot/*.csproj ./ClassifiedAds.Ocelot/
COPY ./ClassifiedAds.Persistence/*.csproj ./ClassifiedAds.Persistence/
COPY ./ClassifiedAds.UnitTests/*.csproj ./ClassifiedAds.UnitTests/
COPY ./ClassifiedAds.WebAPI/*.csproj ./ClassifiedAds.WebAPI/
COPY ./ClassifiedAds.WebMVC/*.csproj ./ClassifiedAds.WebMVC/
COPY ./ClassifiedAds.Monolith.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.BackgroundServer/ClassifiedAds.BackgroundServer.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /ClassifiedAds.Projects
COPY --from=build-env /ClassifiedAds.Projects/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.BackgroundServer.dll"]