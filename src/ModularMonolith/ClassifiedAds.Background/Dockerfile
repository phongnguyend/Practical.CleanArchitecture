FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /ClassifiedAds.ModularMonolith

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.Application/*.csproj ./ClassifiedAds.Application/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/

RUN dotnet restore ./ClassifiedAds.Application/ClassifiedAds.Application.csproj
RUN dotnet restore ./ClassifiedAds.Infrastructure/ClassifiedAds.Infrastructure.csproj

COPY ./ClassifiedAds.Contracts/*.csproj ./ClassifiedAds.Contracts/
COPY ./ClassifiedAds.Modules.AuditLog/*.csproj ./ClassifiedAds.Modules.AuditLog/
COPY ./ClassifiedAds.Modules.Configuration/*.csproj ./ClassifiedAds.Modules.Configuration/
COPY ./ClassifiedAds.Modules.Identity/*.csproj ./ClassifiedAds.Modules.Identity/
COPY ./ClassifiedAds.Modules.Notification/*.csproj ./ClassifiedAds.Modules.Notification/
COPY ./ClassifiedAds.Modules.Product/*.csproj ./ClassifiedAds.Modules.Product/
COPY ./ClassifiedAds.Modules.Storage/*.csproj ./ClassifiedAds.Modules.Storage/

COPY ./ClassifiedAds.Background/*.csproj ./ClassifiedAds.Background/

RUN dotnet restore ./ClassifiedAds.Background/ClassifiedAds.Background.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.Background/ClassifiedAds.Background.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /ClassifiedAds.ModularMonolith
COPY --from=build-env /ClassifiedAds.ModularMonolith/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Background.dll"]