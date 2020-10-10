FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /ClassifiedAds.Services.Identity

# Copy csproj and restore as distinct layers
COPY ./Common/ClassifiedAds.Application/*.csproj ./Common/ClassifiedAds.Application/
COPY ./Common/ClassifiedAds.CrossCuttingConcerns/*.csproj ./Common/ClassifiedAds.CrossCuttingConcerns/
COPY ./Common/ClassifiedAds.Domain/*.csproj ./Common/ClassifiedAds.Domain/
COPY ./Common/ClassifiedAds.Infrastructure/*.csproj ./Common/ClassifiedAds.Infrastructure/
COPY ./Services.Identity/ClassifiedAds.Services.Identity.Api/*.csproj ./Services.Identity/ClassifiedAds.Services.Identity.Api/
COPY ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/*.csproj ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/
RUN dotnet restore ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/ClassifiedAds.Services.Identity.Grpc.csproj

# Copy everything else and build ClassifiedAds
COPY . ./
RUN dotnet publish ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/ClassifiedAds.Services.Identity.Grpc.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /ClassifiedAds.Services.Identity
COPY --from=build-env /ClassifiedAds.Services.Identity/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Services.Identity.Grpc.dll"]