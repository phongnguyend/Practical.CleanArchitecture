FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /ClassifiedAds.Services.Notification

# Copy csproj and restore as distinct layers
COPY ./Common/ClassifiedAds.Application/*.csproj ./Common/ClassifiedAds.Application/
COPY ./Common/ClassifiedAds.CrossCuttingConcerns/*.csproj ./Common/ClassifiedAds.CrossCuttingConcerns/
COPY ./Common/ClassifiedAds.Domain/*.csproj ./Common/ClassifiedAds.Domain/
COPY ./Common/ClassifiedAds.Infrastructure/*.csproj ./Common/ClassifiedAds.Infrastructure/
COPY ./Services.Notification/ClassifiedAds.Services.Notification.Api/*.csproj ./Services.Notification/ClassifiedAds.Services.Notification.Api/
RUN dotnet restore ./Services.Notification/ClassifiedAds.Services.Notification.Api/ClassifiedAds.Services.Notification.Api.csproj

# Copy everything else and build ClassifiedAds
COPY . ./
RUN dotnet publish ./Services.Notification/ClassifiedAds.Services.Notification.Api/ClassifiedAds.Services.Notification.Api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /ClassifiedAds.Services.Notification
COPY --from=build-env /ClassifiedAds.Services.Notification/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Services.Notification.Api.dll"]