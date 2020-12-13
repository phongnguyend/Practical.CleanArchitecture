# Solution Structure
![alt text](/docs/imgs/code-solution-structure-microservices.png)

# Add & Run Database Migration

- Update Connection Strings:

  | Project  | Configuration File | Configuration Key | Db Name |
  | -------- | ------------------ | ----------------- | ------- |
  | Services.AuditLog.Api | [appsettings.json](Services.AuditLog/ClassifiedAds.Services.AuditLog.Api/appsettings.json) | ConnectionStrings:ClassifiedAds | AuditLog
  | Services.AuditLog.Grpc | [appsettings.json](Services.AuditLog/ClassifiedAds.Services.AuditLog.Grpc/appsettings.json) | ConnectionStrings:ClassifiedAds | AuditLog
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