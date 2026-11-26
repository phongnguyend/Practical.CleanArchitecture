FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /source

# Build context: src. Preserve the project-reference directory structure.
COPY Monolith/Directory.Build.props Monolith/global.json Monolith/
COPY Monolith/ClassifiedAds.Application/*.csproj Monolith/ClassifiedAds.Application/
COPY Monolith/ClassifiedAds.CrossCuttingConcerns/*.csproj Monolith/ClassifiedAds.CrossCuttingConcerns/
COPY Monolith/ClassifiedAds.Domain/*.csproj Monolith/ClassifiedAds.Domain/
COPY Monolith/ClassifiedAds.Infrastructure/*.csproj Monolith/ClassifiedAds.Infrastructure/
COPY UIs/blazor/Directory.Build.props UIs/blazor/global.json UIs/blazor/
COPY UIs/blazor/ClassifiedAds.Blazor.Modules/*.csproj UIs/blazor/ClassifiedAds.Blazor.Modules/
COPY UIs/blazor/ClassifiedAds.BlazorServerSide/*.csproj UIs/blazor/ClassifiedAds.BlazorServerSide/
WORKDIR /source/UIs/blazor
RUN dotnet restore ClassifiedAds.BlazorServerSide/ClassifiedAds.BlazorServerSide.csproj

COPY Monolith/ /source/Monolith/
COPY UIs/blazor/ /source/UIs/blazor/
RUN dotnet publish ClassifiedAds.BlazorServerSide/ClassifiedAds.BlazorServerSide.csproj --no-restore -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /ClassifiedAds.Monolith
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "ClassifiedAds.BlazorServerSide.dll"]
