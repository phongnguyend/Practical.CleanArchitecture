FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /ClassifiedAds.Monolith

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.Application/*.csproj ./ClassifiedAds.Application/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/
COPY ./ClassifiedAds.Persistence/*.csproj ./ClassifiedAds.Persistence/

RUN dotnet restore ./ClassifiedAds.Application/ClassifiedAds.Application.csproj
RUN dotnet restore ./ClassifiedAds.Infrastructure/ClassifiedAds.Infrastructure.csproj

COPY ./ClassifiedAds.Blazor.Modules/*.csproj ./ClassifiedAds.Blazor.Modules/
COPY ./ClassifiedAds.BlazorServerSide/*.csproj ./ClassifiedAds.BlazorServerSide/
RUN dotnet restore ./ClassifiedAds.BlazorServerSide/ClassifiedAds.BlazorServerSide.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.BlazorServerSide/ClassifiedAds.BlazorServerSide.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /ClassifiedAds.Monolith
COPY --from=build-env /ClassifiedAds.Monolith/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.BlazorServerSide.dll"]