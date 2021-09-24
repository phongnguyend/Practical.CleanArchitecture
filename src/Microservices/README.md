# Solution Structure
![alt text](/docs/imgs/code-solution-structure-microservices.png)

# Add & Run Database Migration

- Update Connection Strings:

  | Project  | Configuration File | Configuration Key | Db Name |
  | -------- | ------------------ | ----------------- | ------- |
  | Services.AuditLog.Api | [appsettings.json](Services.AuditLog/ClassifiedAds.Services.AuditLog.Api/appsettings.json) | ConnectionStrings:ClassifiedAds | AuditLog
  | Services.AuditLog.Grpc | [appsettings.json](Services.AuditLog/ClassifiedAds.Services.AuditLog.Grpc/appsettings.json) | ConnectionStrings:ClassifiedAds | AuditLog
  | Services.Configuration.Api | [appsettings.json](Services.Configuration/ClassifiedAds.Services.Configuration.Api/appsettings.json) | ConnectionStrings:ClassifiedAds | Configuration
  | Services.Identity.Api | [appsettings.json](Services.Identity/ClassifiedAds.Services.Identity.Api/appsettings.json) | ConnectionStrings:ClassifiedAds | Identity
  | Services.Identity.AuthServer | [appsettings.json](Services.Identity/ClassifiedAds.Services.Identity.AuthServer/appsettings.json) | ConnectionStrings:ClassifiedAds | Identity
  | Services.Identity.Grpc | [appsettings.json](Services.Identity/ClassifiedAds.Services.Identity.Grpc/appsettings.json) | ConnectionStrings:ClassifiedAds | Identity
  | Services.Notification.Api | [appsettings.json](Services.Notification/ClassifiedAds.Services.Notification.Api/appsettings.json) | ConnectionStrings:ClassifiedAds | Notification
  | Services.Notification.Background | [appsettings.json](Services.Notification/ClassifiedAds.Services.Notification.Background/appsettings.json) | ConnectionStrings:ClassifiedAds | Notification
  | Services.Notification.Grpc | [appsettings.json](Services.Notification/ClassifiedAds.Services.Notification.Grpc/appsettings.json) | ConnectionStrings:ClassifiedAds | Notification
  | Services.Product.Api | [appsettings.json](Services.Product/ClassifiedAds.Services.Product.Api/appsettings.json) | ConnectionStrings:ClassifiedAds | Product
  | Services.Storage.Api | [appsettings.json](Services.Storage/ClassifiedAds.Services.Storage.Api/appsettings.json) | ConnectionStrings:ClassifiedAds | Storage


- Run Migration:
  + Option 1: Using dotnet cli:
    + Install **dotnet-ef** cli:
      ```
      dotnet tool install --global dotnet-ef --version="5.0"
      ```
    + Navigate to [AuditLog.Api](Services.AuditLog/ClassifiedAds.Services.AuditLog.Api/) and run these commands:
      ```
      dotnet ef migrations add Init --context AuditLogDbContext -o Migrations/AuditLogDb
      dotnet ef database update --context AuditLogDbContext
      ```
    + Navigate to [Configuration.Api](Services.Configuration/ClassifiedAds.Services.Configuration.Api/) and run these commands:
      ```
      dotnet ef migrations add Init --context ConfigurationDbContext -o Migrations/ConfigurationDb
      dotnet ef database update --context ConfigurationDbContext
      ```
    + Navigate to [Identity.Api](Services.Identity/ClassifiedAds.Services.Identity.Api/) and run these commands:
      ```
      dotnet ef migrations add Init --context IdentityDbContext -o Migrations/IdentityDb
      dotnet ef database update --context IdentityDbContext
      ```
    + Navigate to [Identity.AuthServer](Services.Identity/ClassifiedAds.Services.Identity.AuthServer/) and run these commands:
      ```
      dotnet ef migrations add Init --context ConfigurationDbContext -o Migrations/ConfigurationDb
      dotnet ef migrations add Init --context PersistedGrantDbContext -o Migrations/PersistedGrantDb
      dotnet ef database update --context ConfigurationDbContext
      dotnet ef database update --context PersistedGrantDbContext
      ```
    + Navigate to [Notification.Api](Services.Notification/ClassifiedAds.Services.Notification.Api/) and run these commands:
      ```
      dotnet ef migrations add Init --context NotificationDbContext -o Migrations/NotificationDb
      dotnet ef database update --context NotificationDbContext
      ```
    + Navigate to [Product.Api](Services.Product/ClassifiedAds.Services.Product.Api/) and run these commands:
      ```
      dotnet ef migrations add Init --context ProductDbContext -o Migrations/ProductDb
      dotnet ef database update --context ProductDbContext
      ```
    + Navigate to [Storage.Api](Services.Storage/ClassifiedAds.Services.Storage.Api/) and run these commands:
      ```
      dotnet ef migrations add Init --context StorageDbContext -o Migrations/StorageDb
      dotnet ef database update --context StorageDbContext
      ```
  + Option 2: Using Package Manager Console:
    + Open Package Manager Console
	+ Select **AuditLog.Api** as Default Project and run these commands:
      ```
      Add-Migration -Context AuditLogDbContext Init -OutputDir Migrations/AuditLogDb
      Update-Database -Context AuditLogDbContext
      ```
	+ Select **Configuration.Api** as Default Project and run these commands:
      ```
      Add-Migration -Context ConfigurationDbContext Init -OutputDir Migrations/ConfigurationDb
      Update-Database -Context ConfigurationDbContext
      ```
	+ Select **Identity.Api** as Default Project and run these commands:
      ```
      Add-Migration -Context IdentityDbContext Init -OutputDir Migrations/IdentityDb
      Update-Database -Context IdentityDbContext
      ```
	+ Select **Identity.AuthServer** as Default Project and run these commands:
      ```
      Add-Migration -Context ConfigurationDbContext Init -OutputDir Migrations/ConfigurationDb
      Add-Migration -Context PersistedGrantDbContext Init -OutputDir Migrations/PersistedGrantDb
      Update-Database -Context ConfigurationDbContext
      Update-Database -Context PersistedGrantDbContext
      ```
	+ Select **Notification.Api** as Default Project and run these commands:
      ```
      Add-Migration -Context NotificationDbContext Init -OutputDir Migrations/NotificationDb
      Update-Database -Context NotificationDbContext
      ```
	+ Select **Product.Api** as Default Project and run these commands:
      ```
      Add-Migration -Context ProductDbContext Init -OutputDir Migrations/ProductDb
      Update-Database -Context ProductDbContext
      ```
	+ Select **Storage.Api** as Default Project and run these commands:
      ```
      Add-Migration -Context StorageDbContext Init -OutputDir Migrations/StorageDb
      Update-Database -Context StorageDbContext
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
  docker tag classifiedads.gateways.webapi phongnguyend/classifiedads.gateways.webapi
  docker tag classifiedads.services.auditlog.api phongnguyend/classifiedads.services.auditlog.api
  docker tag classifiedads.services.auditlog.grpc phongnguyend/classifiedads.services.auditlog.grpc
  docker tag classifiedads.services.configuration.api phongnguyend/classifiedads.services.configuration.api
  docker tag classifiedads.services.identity.api phongnguyend/classifiedads.services.identity.api
  docker tag classifiedads.services.identity.authserver phongnguyend/classifiedads.services.identity.authserver
  docker tag classifiedads.services.identity.grpc phongnguyend/classifiedads.services.identity.grpc
  docker tag classifiedads.services.notification.api phongnguyend/classifiedads.services.notification.api
  docker tag classifiedads.services.notification.background phongnguyend/classifiedads.services.notification.background
  docker tag classifiedads.services.notification.grpc phongnguyend/classifiedads.services.notification.grpc
  docker tag classifiedads.services.product.api phongnguyend/classifiedads.services.product.api
  docker tag classifiedads.services.storage.api phongnguyend/classifiedads.services.storage.api
  ```

- Push
  ```
  docker push phongnguyend/classifiedads.gateways.webapi
  docker push phongnguyend/classifiedads.services.auditlog.api
  docker push phongnguyend/classifiedads.services.auditlog.grpc
  docker push phongnguyend/classifiedads.services.configuration.api
  docker push phongnguyend/classifiedads.services.identity.api
  docker push phongnguyend/classifiedads.services.identity.authserver
  docker push phongnguyend/classifiedads.services.identity.grpc
  docker push phongnguyend/classifiedads.services.notification.api
  docker push phongnguyend/classifiedads.services.notification.background
  docker push phongnguyend/classifiedads.services.notification.grpc
  docker push phongnguyend/classifiedads.services.product.api
  docker push phongnguyend/classifiedads.services.storage.api
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
  helm install myrelease .helm/microservices
  helm list
  helm upgrade myrelease .helm/microservices
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
  dotnet restore ClassifiedAds.Microservices.sln

  dotnet build -p:Version=1.0.0.1 -c Release

  dotnet publish -p:Version=1.0.0.1 -c Release ./Gateways.WebAPI/ClassifiedAds.Gateways.WebAPI/ClassifiedAds.Gateways.WebAPI.csproj -o ./publish/ClassifiedAds.Gateways.WebAPI
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.AuditLog/ClassifiedAds.Services.AuditLog.Api/ClassifiedAds.Services.AuditLog.Api.csproj -o ./publish/ClassifiedAds.Services.AuditLog.Api
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.AuditLog/ClassifiedAds.Services.AuditLog.Grpc/ClassifiedAds.Services.AuditLog.Grpc.csproj -o ./publish/ClassifiedAds.Services.AuditLog.Grpc
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Configuration/ClassifiedAds.Services.Configuration.Api/ClassifiedAds.Services.Configuration.Api.csproj -o ./publish/ClassifiedAds.Services.Configuration.Api
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Identity/ClassifiedAds.Services.Identity.Api/ClassifiedAds.Services.Identity.Api.csproj -o ./publish/ClassifiedAds.Services.Identity.Api
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Identity/ClassifiedAds.Services.Identity.AuthServer/ClassifiedAds.Services.Identity.AuthServer.csproj -o ./publish/ClassifiedAds.Services.Identity.AuthServer
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Identity/ClassifiedAds.Services.Identity.Grpc/ClassifiedAds.Services.Identity.Grpc.csproj -o ./publish/ClassifiedAds.Services.Identity.Grpc
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Notification/ClassifiedAds.Services.Notification.Api/ClassifiedAds.Services.Notification.Api.csproj -o ./publish/ClassifiedAds.Services.Notification.Api
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Notification/ClassifiedAds.Services.Notification.Background/ClassifiedAds.Services.Notification.Background.csproj -o ./publish/ClassifiedAds.Services.Notification.Background
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Notification/ClassifiedAds.Services.Notification.Grpc/ClassifiedAds.Services.Notification.Grpc.csproj -o ./publish/ClassifiedAds.Services.Notification.Grpc
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Product/ClassifiedAds.Services.Product.Api/ClassifiedAds.Services.Product.Api.csproj -o ./publish/ClassifiedAds.Services.Product.Api
  dotnet publish -p:Version=1.0.0.1 -c Release ./Services.Storage/ClassifiedAds.Services.Storage.Api/ClassifiedAds.Services.Storage.Api.csproj -o ./publish/ClassifiedAds.Services.Storage.Api
  ```

- Pack
  ```
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Gateways.WebAPI --basePath=./publish/ClassifiedAds.Gateways.WebAPI
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.AuditLog.Api --basePath=./publish/ClassifiedAds.Services.AuditLog.Api
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.AuditLog.Grpc --basePath=./publish/ClassifiedAds.Services.AuditLog.Grpc
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Configuration.Api --basePath=./publish/ClassifiedAds.Services.Configuration.Api
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Identity.Api --basePath=./publish/ClassifiedAds.Services.Identity.Api
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Identity.AuthServer --basePath=./publish/ClassifiedAds.Services.Identity.AuthServer
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Identity.Grpc --basePath=./publish/ClassifiedAds.Services.Identity.Grpc
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Notification.Api --basePath=./publish/ClassifiedAds.Services.Notification.Api
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Notification.Background --basePath=./publish/ClassifiedAds.Services.Notification.Background
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Notification.Grpc --basePath=./publish/ClassifiedAds.Services.Notification.Grpc
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Product.Api --basePath=./publish/ClassifiedAds.Services.Product.Api
  dotnet octo pack --version=1.0.0.1 --outFolder=./publish --overwrite --id=ClassifiedAds.Services.Storage.Api --basePath=./publish/ClassifiedAds.Services.Storage.Api
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
  dotnet sonarscanner begin /v:"1.0.0" /d:sonar.host.url="https://sonarcloud.io" /o:"phongnguyend" /k:"Microservices" /d:sonar.login="<token>"
  dotnet build ClassifiedAds.Microservices.sln
  dotnet sonarscanner end /d:sonar.login="<token>"
  ```
