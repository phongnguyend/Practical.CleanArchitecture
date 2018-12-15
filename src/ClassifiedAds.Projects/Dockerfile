FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /ClassifiedAds.Projects

# Copy csproj and restore as distinct layers
COPY ./ClassifiedAds.ApplicationServices/*.csproj ./ClassifiedAds.ApplicationServices/
COPY ./ClassifiedAds.CrossCuttingConcerns/*.csproj ./ClassifiedAds.CrossCuttingConcerns/
COPY ./ClassifiedAds.Domain/*.csproj ./ClassifiedAds.Domain/
COPY ./ClassifiedAds.DomainServices/*.csproj ./ClassifiedAds.DomainServices/
COPY ./ClassifiedAds.Infrastructure/*.csproj ./ClassifiedAds.Infrastructure/
COPY ./ClassifiedAds.Persistance/*.csproj ./ClassifiedAds.Persistance/
COPY ./ClassifiedAds.Web/*.csproj ./ClassifiedAds.Web/
COPY ./ClassifiedAds.UnitTest/*.csproj ./ClassifiedAds.UnitTest/
COPY ./ClassifiedAds.Projects.sln .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./ClassifiedAds.Web/ClassifiedAds.Web.csproj -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /ClassifiedAds.Web
COPY --from=build-env /ClassifiedAds.Projects/ClassifiedAds.Web/out .

ENTRYPOINT ["dotnet", "ClassifiedAds.Web.dll"]