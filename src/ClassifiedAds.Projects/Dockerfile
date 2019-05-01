FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /ClassifiedAds.Projects

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.ApplicationServices/*.csproj ./ClassifiedAds.ApplicationServices/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.DomainServices/*.csproj ./ClassifiedAds.DomainServices/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/
COPY ./ClassifiedAds.Persistance/*.csproj ./ClassifiedAds.Persistance/
COPY ./ClassifiedAds.WebMVC/*.csproj ./ClassifiedAds.WebMVC/
COPY ./ClassifiedAds.UnitTests/*.csproj ./ClassifiedAds.UnitTests/
COPY ./ClassifiedAds.Projects.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.WebMVC/ClassifiedAds.WebMVC.csproj -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /ClassifiedAds.WebMVC
COPY --from=build-env /ClassifiedAds.Projects/ClassifiedAds.WebMVC/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.WebMVC.dll"]