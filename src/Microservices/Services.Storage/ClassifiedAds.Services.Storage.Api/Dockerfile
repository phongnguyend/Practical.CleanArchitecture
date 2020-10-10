FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /ClassifiedAds.Services.Storage

# Copy csproj and restore as distinct layers
COPY ./Common/ClassifiedAds.Application/*.csproj ./Common/ClassifiedAds.Application/
COPY ./Common/ClassifiedAds.CrossCuttingConcerns/*.csproj ./Common/ClassifiedAds.CrossCuttingConcerns/
COPY ./Common/ClassifiedAds.Domain/*.csproj ./Common/ClassifiedAds.Domain/
COPY ./Common/ClassifiedAds.Infrastructure/*.csproj ./Common/ClassifiedAds.Infrastructure/
COPY ./Services.Storage/ClassifiedAds.Services.Storage.Api/*.csproj ./Services.Storage/ClassifiedAds.Services.Storage.Api/
RUN dotnet restore ./Services.Storage/ClassifiedAds.Services.Storage.Api/ClassifiedAds.Services.Storage.Api.csproj

# Copy everything else and build ClassifiedAds
COPY . ./
RUN dotnet publish ./Services.Storage/ClassifiedAds.Services.Storage.Api/ClassifiedAds.Services.Storage.Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /ClassifiedAds.Services.Storage
COPY --from=build-env /ClassifiedAds.Services.Storage/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Services.Storage.Api.dll"]