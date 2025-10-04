FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /ClassifiedAds.ModularMonolith

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.Application/*.csproj ./ClassifiedAds.Application/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/
COPY ./ClassifiedAds.Persistence.SqlServer/*.csproj ./ClassifiedAds.Persistence.SqlServer/

RUN dotnet restore ./ClassifiedAds.Application/ClassifiedAds.Application.csproj
RUN dotnet restore ./ClassifiedAds.Infrastructure/ClassifiedAds.Infrastructure.csproj
RUN dotnet restore ./ClassifiedAds.Persistence.SqlServer/ClassifiedAds.Persistence.SqlServer.csproj

COPY ./ClassifiedAds.Contracts/*.csproj ./ClassifiedAds.Contracts/
COPY ./ClassifiedAds.Modules.AuditLog/*.csproj ./ClassifiedAds.Modules.AuditLog/
COPY ./ClassifiedAds.Modules.Configuration/*.csproj ./ClassifiedAds.Modules.Configuration/
COPY ./ClassifiedAds.Modules.Identity/*.csproj ./ClassifiedAds.Modules.Identity/
COPY ./ClassifiedAds.Modules.Notification/*.csproj ./ClassifiedAds.Modules.Notification/
COPY ./ClassifiedAds.Modules.Product/*.csproj ./ClassifiedAds.Modules.Product/
COPY ./ClassifiedAds.Modules.Storage/*.csproj ./ClassifiedAds.Modules.Storage/

COPY ./ClassifiedAds.WebAPI/*.csproj ./ClassifiedAds.WebAPI/

RUN dotnet restore ./ClassifiedAds.WebAPI/ClassifiedAds.WebAPI.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.WebAPI/ClassifiedAds.WebAPI.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /ClassifiedAds.ModularMonolith
COPY --from=build-env /ClassifiedAds.ModularMonolith/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.WebAPI.dll"]