FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /ClassifiedAds.Microservices

# Copy csproj and restore as distinct layers
COPY ./Gateways.WebAPI/ClassifiedAds.Gateways.WebAPI/*.csproj ./Gateways.WebAPI/ClassifiedAds.Gateways.WebAPI/
RUN dotnet restore ./Gateways.WebAPI/ClassifiedAds.Gateways.WebAPI/ClassifiedAds.Gateways.WebAPI.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish ./Gateways.WebAPI/ClassifiedAds.Gateways.WebAPI/ClassifiedAds.Gateways.WebAPI.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /ClassifiedAds.Microservices
COPY --from=build-env /ClassifiedAds.Microservices/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Gateways.WebAPI.dll"]