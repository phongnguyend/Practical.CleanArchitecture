FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /ClassifiedAds.Projects

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.ApplicationServices/*.csproj ./ClassifiedAds.ApplicationServices/
COPY ./ClassifiedAds.ApplicationServices.DI/*.csproj ./ClassifiedAds.ApplicationServices.DI/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.DomainServices/*.csproj ./ClassifiedAds.DomainServices/
COPY ./ClassifiedAds.DomainServices.DI/*.csproj ./ClassifiedAds.DomainServices.DI/
COPY ./ClassifiedAds.DomainServices.UnitTests/*.csproj ./ClassifiedAds.DomainServices.UnitTests/
COPY ./ClassifiedAds.Identity/*.csproj ./ClassifiedAds.Identity/
COPY ./ClassifiedAds.IdentityServer/*.csproj ./ClassifiedAds.IdentityServer/
COPY ./ClassifiedAds.IdentityServer.Persistence/*.csproj ./ClassifiedAds.IdentityServer.Persistence/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/
COPY ./ClassifiedAds.Persistence/*.csproj ./ClassifiedAds.Persistence/
COPY ./ClassifiedAds.Persistence.DI/*.csproj ./ClassifiedAds.Persistence.DI/
COPY ./ClassifiedAds.WebAPI/*.csproj ./ClassifiedAds.WebAPI/
COPY ./ClassifiedAds.WebAPI.IntegrationTests/*.csproj ./ClassifiedAds.WebAPI.IntegrationTests/
COPY ./ClassifiedAds.WebMVC/*.csproj ./ClassifiedAds.WebMVC/
COPY ./ClassifiedAds.WebMVC.AutomationTests/*.csproj ./ClassifiedAds.WebMVC.AutomationTests/
COPY ./ClassifiedAds.Projects.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.WebAPI/ClassifiedAds.WebAPI.csproj -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /ClassifiedAds.WebAPI
COPY --from=build-env /ClassifiedAds.Projects/ClassifiedAds.WebAPI/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.WebAPI.dll"]