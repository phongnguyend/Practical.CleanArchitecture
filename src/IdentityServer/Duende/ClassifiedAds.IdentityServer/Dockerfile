FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /ClassifiedAds.IdentityServer

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.Application/*.csproj ./ClassifiedAds.Application/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.IdentityServer/*.csproj ./ClassifiedAds.IdentityServer/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/
COPY ./ClassifiedAds.Migrator/*.csproj ./ClassifiedAds.Migrator/
COPY ./ClassifiedAds.Persistence/*.csproj ./ClassifiedAds.Persistence/
COPY ./ClassifiedAds.Duende.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.IdentityServer/ClassifiedAds.IdentityServer.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /ClassifiedAds.IdentityServer
COPY --from=build-env /ClassifiedAds.IdentityServer/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.IdentityServer.dll"]