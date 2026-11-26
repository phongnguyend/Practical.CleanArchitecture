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
COPY UIs/blazor/ClassifiedAds.BlazorWebAssembly/*.csproj UIs/blazor/ClassifiedAds.BlazorWebAssembly/
WORKDIR /source/UIs/blazor
RUN dotnet restore ClassifiedAds.BlazorWebAssembly/ClassifiedAds.BlazorWebAssembly.csproj

COPY Monolith/ /source/Monolith/
COPY UIs/blazor/ /source/UIs/blazor/
RUN dotnet publish ClassifiedAds.BlazorWebAssembly/ClassifiedAds.BlazorWebAssembly.csproj --no-restore -c Release -o /out

FROM nginx:alpine
COPY --from=build-env /out/wwwroot /usr/share/nginx/html
COPY UIs/blazor/ClassifiedAds.BlazorWebAssembly/nginx.conf /etc/nginx/conf.d/default.conf
COPY UIs/blazor/ClassifiedAds.BlazorWebAssembly/appsettings.template.json /etc/blazor/appsettings.template.json
COPY UIs/blazor/ClassifiedAds.BlazorWebAssembly/40-appsettings.sh /docker-entrypoint.d/40-appsettings.sh
RUN chmod +x /docker-entrypoint.d/40-appsettings.sh
ENV IDENTITY_AUTHORITY=http://host.docker.internal:9000 \
    API_ENDPOINT=http://host.docker.internal:9002 \
    WASM_ORIGIN=http://localhost:9009
