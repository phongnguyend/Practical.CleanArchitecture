FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /ClassifiedAds.Services.Product

# Copy csproj and restore as distinct layers
COPY ./Common/ClassifiedAds.Application/*.csproj ./Common/ClassifiedAds.Application/
COPY ./Common/ClassifiedAds.CrossCuttingConcerns/*.csproj ./Common/ClassifiedAds.CrossCuttingConcerns/
COPY ./Common/ClassifiedAds.Domain/*.csproj ./Common/ClassifiedAds.Domain/
COPY ./Common/ClassifiedAds.Infrastructure/*.csproj ./Common/ClassifiedAds.Infrastructure/
COPY ./Services.Product/ClassifiedAds.Services.Product.Api/*.csproj ./Services.Product/ClassifiedAds.Services.Product.Api/
RUN dotnet restore ./Services.Product/ClassifiedAds.Services.Product.Api/ClassifiedAds.Services.Product.Api.csproj

# Copy everything else and build ClassifiedAds
COPY . ./
RUN dotnet publish ./Services.Product/ClassifiedAds.Services.Product.Api/ClassifiedAds.Services.Product.Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /ClassifiedAds.Services.Product
COPY --from=build-env /ClassifiedAds.Services.Product/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Services.Product.Api.dll"]