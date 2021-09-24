# Solution Structure
![alt text](/docs/imgs/code-solution-structure-modular-monolith.png)

# Add & Run Database Migration

- Update Connection Strings:

  | Project  | Configuration File | Configuration Key |
  | -------- | ------------------ | ----------------- |
  | ClassifiedAds.Migrator | [appsettings.json](ClassifiedAds.Migrator/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.BackgroundServer | [appsettings.json](ClassifiedAds.BackgroundServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.IdentityServer | [appsettings.json](ClassifiedAds.IdentityServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.WebAPI | [appsettings.json](ClassifiedAds.WebAPI/appsettings.json) | ConnectionStrings:ClassifiedAds |


- Run Migration:
  + Option 1: Using dotnet cli:
    + Install **dotnet-ef** cli:
      ```
      dotnet tool install --global dotnet-ef --version="5.0"
      ```
    + Navigate to [ClassifiedAds.Migrator](ClassifiedAds.Migrator/) and run these commands:
      ```
      dotnet ef migrations add Init --context AuditLogDbContext -o Migrations/AuditLogDb
      dotnet ef migrations add Init --context ClassifiedAds.Modules.Configuration.Repositories.ConfigurationDbContext -o Migrations/ConfigurationDb
      dotnet ef migrations add Init --context IdentityDbContext -o Migrations/IdentityDb
      dotnet ef migrations add Init --context NotificationDbContext -o Migrations/NotificationDb
      dotnet ef migrations add Init --context ProductDbContext -o Migrations/ProductDb
      dotnet ef migrations add Init --context StorageDbContext -o Migrations/StorageDb
      dotnet ef migrations add Init --context IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext -o Migrations/Id4ConfigurationDb
      dotnet ef migrations add Init --context IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext -o Migrations/Id4PersistedGrantDb
      dotnet ef database update --context AuditLogDbContext
      dotnet ef database update --context ClassifiedAds.Modules.Configuration.Repositories.ConfigurationDbContext
      dotnet ef database update --context IdentityDbContext
      dotnet ef database update --context NotificationDbContext
      dotnet ef database update --context ProductDbContext
      dotnet ef database update --context StorageDbContext
      dotnet ef database update --context IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext
      dotnet ef database update --context IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext

      ```
  + Option 2: Using Package Manager Console:
    + Set **ClassifiedAds.Migrator** as StartUp Project
    + Open Package Manager Console, select **ClassifiedAds.Migrator** as Default Project
    + Run these commands:
      ```
      Add-Migration -Context AuditLogDbContext Init -OutputDir Migrations/AuditLogDb
      Add-Migration -Context ClassifiedAds.Modules.Configuration.Repositories.ConfigurationDbContext Init -OutputDir Migrations/ConfigurationDb
      Add-Migration -Context IdentityDbContext Init -OutputDir Migrations/IdentityDb
      Add-Migration -Context NotificationDbContext Init -OutputDir Migrations/NotificationDb
      Add-Migration -Context ProductDbContext Init -OutputDir Migrations/ProductDb
      Add-Migration -Context StorageDbContext Init -OutputDir Migrations/StorageDb
      Add-Migration -Context IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext Init -OutputDir Migrations/Id4ConfigurationDb
      Add-Migration -Context IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext Init -OutputDir Migrations/Id4PersistedGrantDb
      Update-Database -Context AuditLogDbContext
      Update-Database -Context ClassifiedAds.Modules.Configuration.Repositories.ConfigurationDbContext
      Update-Database -Context IdentityDbContext
      Update-Database -Context NotificationDbContext
      Update-Database -Context ProductDbContext
      Update-Database -Context StorageDbContext
      Update-Database -Context IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext
      Update-Database -Context IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext

      ```  

# Build & Run Locally using Tye

- Install Tye
  ```
  dotnet tool install -g Microsoft.Tye --version "0.5.0-alpha.20555.1"
  dotnet tool update -g Microsoft.Tye --version "0.5.0-alpha.20555.1"
  ```
  
- Run
  ```
  tye run
  ```
  
- Open Tye Dashboard: http://localhost:8000/

# Build & Deploy to Kubernetes

- Build
  ```
  docker-compose build
  ```

- Tag
  ```
  docker tag classifiedads.modularmonolith.backgroundserver phongnguyend/classifiedads.modularmonolith.backgroundserver
  docker tag classifiedads.modularmonolith.migrator phongnguyend/classifiedads.modularmonolith.migrator
  docker tag classifiedads.modularmonolith.webapi phongnguyend/classifiedads.modularmonolith.webapi
  docker tag classifiedads.modularmonolith.identityserver phongnguyend/classifiedads.modularmonolith.identityserver
  ```

- Push
  ```
  docker push phongnguyend/classifiedads.modularmonolith.backgroundserver
  docker push phongnguyend/classifiedads.modularmonolith.migrator
  docker push phongnguyend/classifiedads.modularmonolith.webapi
  docker push phongnguyend/classifiedads.modularmonolith.identityserver
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
  helm install myrelease .helm/modularmonolith
  helm list
  helm upgrade myrelease .helm/modularmonolith
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
  dotnet restore ClassifiedAds.ModularMonolith.sln

  dotnet build -p:Version=1.0.0.1 -c Release

  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.BackgroundServer/ClassifiedAds.BackgroundServer.csproj -o ./publish/ClassifiedAds.BackgroundServer
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.IdentityServer/ClassifiedAds.IdentityServer.csproj -o ./publish/ClassifiedAds.IdentityServer
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.Migrator/ClassifiedAds.Migrator.csproj -o ./publish/ClassifiedAds.Migrator
  dotnet publish -p:Version=1.0.0.1 -c Release ./ClassifiedAds.WebAPI/ClassifiedAds.WebAPI.csproj -o ./publish/ClassifiedAds.WebAPI
  ```

- Pack
  ```
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.BackgroundServer --basePath=./publish/ClassifiedAds.BackgroundServer
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.IdentityServer --basePath=./publish/ClassifiedAds.IdentityServer
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Migrator --basePath=./publish/ClassifiedAds.Migrator
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.WebAPI --basePath=./publish/ClassifiedAds.WebAPI
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
  dotnet sonarscanner begin /v:"1.0.0" /d:sonar.host.url="https://sonarcloud.io" /o:"phongnguyend" /k:"ModularMonolith" /d:sonar.login="<token>"
  dotnet build ClassifiedAds.ModularMonolith.sln
  dotnet sonarscanner end /d:sonar.login="<token>"
  ```
