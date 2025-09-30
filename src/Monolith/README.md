# Solution Structure
![alt text](/docs/imgs/code-solution-structure.png)

# Add & Run Database Migration

- Update Connection Strings:

  | Project  | Configuration File | Configuration Key |
  | -------- | ------------------ | ----------------- |
  | ClassifiedAds.Migrator | [appsettings.json](ClassifiedAds.Migrator/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.Background | [appsettings.json](ClassifiedAds.Background/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.IdentityServer | [appsettings.json](../IdentityServers/OpenIddict/ClassifiedAds.IdentityServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.WebAPI | [appsettings.json](ClassifiedAds.WebAPI/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.WebMVC | [appsettings.json](ClassifiedAds.WebMVC/appsettings.json) | ConnectionStrings:ClassifiedAds |


- Run Migration:
  + Option 1: Using dotnet cli:
    + Install **dotnet-ef** cli:
      ```
      dotnet tool install --global dotnet-ef --version="8.0"
      ```
    + Navigate to [ClassifiedAds.Migrator](ClassifiedAds.Migrator/) and run these commands:
      ```
      dotnet ef migrations add Init --context AdsDbContext -o Migrations/AdsDb
      dotnet ef database update --context AdsDbContext
      ```
  + Option 2: Using Package Manager Console:
    + Set **ClassifiedAds.Migrator** as StartUp Project
    + Open Package Manager Console, select **ClassifiedAds.Migrator** as Default Project
    + Run these commands:
      ```
      Add-Migration -Context AdsDbContext Init -OutputDir Migrations/AdsDb
      Update-Database -Context AdsDbContext
      ```  

# Build & Run Locally with Aspire

- Run
  ```
  dotnet build
  dotnet run --project ClassifiedAds.AspireAppHost
  ```
  
- Open Aspire Dashboard: https://localhost:17063/login?t=xxx

# Build & Deploy to Kubernetes

- Build
  ```
  docker compose build
  ```

- Tag
  ```
  docker tag classifiedads.background phongnguyend/classifiedads.background
  docker tag classifiedads.migrator phongnguyend/classifiedads.migrator
  docker tag classifiedads.webapi phongnguyend/classifiedads.webapi
  docker tag classifiedads.blazor phongnguyend/classifiedads.blazor
  docker tag classifiedads.identityserver phongnguyend/classifiedads.identityserver
  docker tag classifiedads.webmvc phongnguyend/classifiedads.webmvc
  ```

- Push
  ```
  docker push phongnguyend/classifiedads.background
  docker push phongnguyend/classifiedads.migrator
  docker push phongnguyend/classifiedads.webapi
  docker push phongnguyend/classifiedads.blazor
  docker push phongnguyend/classifiedads.identityserver
  docker push phongnguyend/classifiedads.webmvc
  ```

- Apply
  ```
  kubectl apply -f .k8s
  kubectl get all
  kubectl get services
  kubectl get pods
  ```

- Delete
  ```
  kubectl delete -f .k8s
  ```
  
- Use Helm
  ```
  helm install myrelease .helm/monolith
  helm list
  helm upgrade myrelease .helm/monolith
  ```

- UnInstall
  ```
  helm uninstall myrelease
  ```

# Build Nuget Packages using OctoPack

- Install OctoPack
  ```
  dotnet tool install Octopus.DotNet.Cli --global --version 4.39.1
  dotnet octo --version
  dotnet tool update Octopus.DotNet.Cli --global
  dotnet tool uninstall Octopus.DotNet.Cli --global
  dotnet tool install Octopus.DotNet.Cli --global --version <version>
  ```

- Build
  ```
  dotnet restore ClassifiedAds.Monolith.slnx

  dotnet build -p:Version=1.0.0.1 -c Release

  dotnet publish -p:Version=1.0.0.1 -c Release ../IdentityServers/OpenIddict/ClassifiedAds.IdentityServer/ClassifiedAds.IdentityServer.csproj -o ./publish/ClassifiedAds.IdentityServer
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.Background/ClassifiedAds.Background.csproj -o ./publish/ClassifiedAds.Background
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.Migrator/ClassifiedAds.Migrator.csproj -o ./publish/ClassifiedAds.Migrator
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.WebAPI/ClassifiedAds.WebAPI.csproj -o ./publish/ClassifiedAds.WebAPI
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.BlazorServerSide/ClassifiedAds.BlazorServerSide.csproj -o ./publish/ClassifiedAds.BlazorServerSide
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.BlazorWebAssembly/ClassifiedAds.BlazorWebAssembly.csproj -o ./publish/ClassifiedAds.BlazorWebAssembly
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.WebMVC/ClassifiedAds.WebMVC.csproj -o ./publish/ClassifiedAds.WebMVC
  ```

- Pack
  ```
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.IdentityServer --basePath=./publish/ClassifiedAds.IdentityServer
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Background --basePath=./publish/ClassifiedAds.Background
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Migrator --basePath=./publish/ClassifiedAds.Migrator
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.WebAPI --basePath=./publish/ClassifiedAds.WebAPI
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.BlazorServerSide --basePath=./publish/ClassifiedAds.BlazorServerSide
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.BlazorWebAssembly --basePath=./publish/ClassifiedAds.BlazorWebAssembly
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.WebMVC --basePath=./publish/ClassifiedAds.WebMVC
  ```

# SonarQube

- Install Sonar Scanner
  ```
  dotnet tool install --global dotnet-sonarscanner
  dotnet tool list --global
  java --version
  ```

- Build & Scan
  ```
  dotnet sonarscanner begin /v:"1.0.0" /d:sonar.host.url="https://sonarcloud.io" /o:"phongnguyend" /k:"Monolith" /d:sonar.login="<token>"
  dotnet build ClassifiedAds.Monolith.slnx
  dotnet sonarscanner end /d:sonar.login="<token>"
  ```
