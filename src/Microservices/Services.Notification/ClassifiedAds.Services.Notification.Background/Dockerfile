FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /ClassifiedAds.Microservices

# Copy csproj and restore as distinct layers
COPY ./Common/ClassifiedAds.Application/*.csproj ./Common/ClassifiedAds.Application/
COPY ./Common/ClassifiedAds.CrossCuttingConcerns/*.csproj ./Common/ClassifiedAds.CrossCuttingConcerns/
COPY ./Common/ClassifiedAds.Domain/*.csproj ./Common/ClassifiedAds.Domain/
COPY ./Common/ClassifiedAds.Infrastructure/*.csproj ./Common/ClassifiedAds.Infrastructure/
RUN dotnet restore ./Common/ClassifiedAds.Application/ClassifiedAds.Application.csproj
RUN dotnet restore ./Common/ClassifiedAds.Infrastructure/ClassifiedAds.Infrastructure.csproj

COPY ./Services.Notification/ClassifiedAds.Services.Notification/*.csproj ./Services.Notification/ClassifiedAds.Services.Notification/
RUN dotnet restore ./Services.Notification/ClassifiedAds.Services.Notification/ClassifiedAds.Services.Notification.csproj

COPY ./Services.Notification/ClassifiedAds.Services.Notification.Background/*.csproj ./Services.Notification/ClassifiedAds.Services.Notification.Background/
RUN dotnet restore ./Services.Notification/ClassifiedAds.Services.Notification.Background/ClassifiedAds.Services.Notification.Background.csproj

# Copy everything else and build ClassifiedAds
COPY . ./
RUN dotnet publish ./Services.Notification/ClassifiedAds.Services.Notification.Background/ClassifiedAds.Services.Notification.Background.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /ClassifiedAds.Microservices
COPY --from=build-env /ClassifiedAds.Microservices/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Services.Notification.Background.dll"]