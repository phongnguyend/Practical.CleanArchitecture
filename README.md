[Clean Architecture: Patterns, Practices, and Principles | Matthew Renze | Pluralsight](https://app.pluralsight.com/library/courses/clean-architecture-patterns-practices-principles/table-of-contents)

# Database Centric vs Domain Centric Architecture 
![alt text](/docs/imgs/database-centrics-vs-domain-centric-architecture.png)

[*(open on draw.io)*](https://www.draw.io/#Uhttps%3A%2F%2Fraw.githubusercontent.com%2Fphongnguyend%2FPractical.CleanArchitecture%2Fmaster%2Fdocs%2Fimgs%2Fdatabase-centrics-vs-domain-centric-architecture.drawio)

# Hexagonal Architecture
![alt text](/docs/imgs/hexagonal-architecture.png)

[*(open on draw.io)*](https://www.draw.io/#Uhttps%3A%2F%2Fraw.githubusercontent.com%2Fphongnguyend%2FPractical.CleanArchitecture%2Fmaster%2Fdocs%2Fimgs%2Fhexagonal-architecture.drawio)

# Onion Architecture
![alt text](/docs/imgs/onion-architecture.png)

[*(open on draw.io)*](https://www.draw.io/#Uhttps%3A%2F%2Fraw.githubusercontent.com%2Fphongnguyend%2FPractical.CleanArchitecture%2Fmaster%2Fdocs%2Fimgs%2Fonion-architecture.drawio)

# The Clean Architecture
![alt text](/docs/imgs/the-clean-architecture.png)

[*(open on draw.io)*](https://www.draw.io/#Uhttps%3A%2F%2Fraw.githubusercontent.com%2Fphongnguyend%2FPractical.CleanArchitecture%2Fmaster%2Fdocs%2Fimgs%2Fthe-clean-architecture.drawio)

# Classic Three-layer Architecture
![alt text](/docs/imgs/classic-three-layer-architecture.png)

[*(open on draw.io)*](https://www.draw.io/#Uhttps%3A%2F%2Fraw.githubusercontent.com%2Fphongnguyend%2FPractical.CleanArchitecture%2Fmaster%2Fdocs%2Fimgs%2Fclassic-three-layer-architecture.drawio)

# Modern Four-layer Architecture
![alt text](/docs/imgs/modern-four-layer-architecture.png)

[*(open on draw.io)*](https://www.draw.io/#Uhttps%3A%2F%2Fraw.githubusercontent.com%2Fphongnguyend%2FPractical.CleanArchitecture%2Fmaster%2Fdocs%2Fimgs%2Fmodern-four-layer-architecture.drawio)

# Layer Dependencies
![alt text](/docs/imgs/layer-dependencies.png)

[*(open on draw.io)*](https://www.draw.io/#Uhttps%3A%2F%2Fraw.githubusercontent.com%2Fphongnguyend%2FPractical.CleanArchitecture%2Fmaster%2Fdocs%2Fimgs%2Flayer-dependencies.drawio)

# Layer Examples
![alt text](/docs/imgs/layer-examples.png)

[*(open on draw.io)*](https://www.draw.io/#Uhttps%3A%2F%2Fraw.githubusercontent.com%2Fphongnguyend%2FPractical.CleanArchitecture%2Fmaster%2Fdocs%2Fimgs%2Flayer-examples.drawio)

# Solution Structure
![alt text](/docs/imgs/code-solution-structure.png)

# How to Run:

## Configure Database

- Update Connection Strings:

  | Project  | Configuration File | Configuration Key |
  | -------- | ------------------ | ----------------- |
  | ClassifiedAds.Migrator | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.Migrator/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.BackgroundServer | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.BackgroundServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.GRPC | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.GRPC/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.IdentityServer | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.IdentityServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.WebAPI | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebAPI/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.WebMVC | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebMVC/appsettings.json) | ConnectionStrings:ClassifiedAds |


- Run Migration:
  + Option 1: Using dotnet cli:
    + Install **dotnet-ef** cli:
      ```
      dotnet tool install --global dotnet-ef --version="3.1"
      ```
    + Navigate to [ClassifiedAds.Migrator](/src/ClassifiedAds.Projects/ClassifiedAds.Migrator/) and run these commands:
      ```
      dotnet ef migrations add Init --context AdsDbContext -o Migrations/AdsDb
      dotnet ef migrations add Init --context ConfigurationDbContext -o Migrations/ConfigurationDb
      dotnet ef migrations add Init --context PersistedGrantDbContext -o Migrations/PersistedGrantDb
      dotnet ef database update --context AdsDbContext
      dotnet ef database update --context ConfigurationDbContext
      dotnet ef database update --context PersistedGrantDbContext
      ```
  + Option 2: Using Package Manager Console:
    + Set **ClassifiedAds.Migrator** as StartUp Project
    + Open Package Manager Console, select **ClassifiedAds.Migrator** as Default Project
    + Run these commands:
      ```
      Add-Migration -Context AdsDbContext Init -OutputDir Migrations/AdsDb
      Add-Migration -Context ConfigurationDbContext Init -OutputDir Migrations/ConfigurationDb
      Add-Migration -Context PersistedGrantDbContext Init -OutputDir Migrations/PersistedGrantDb
      Update-Database -Context AdsDbContext
      Update-Database -Context ConfigurationDbContext
      Update-Database -Context PersistedGrantDbContext
      ```

## Configure Storage

- Open [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebMVC/appsettings.json) and jump to **Storage** section.
  ```js
  "Storage": {
    "Provider": "Local",
  },
  ```

- Use Local Files:
  ```js
  "Storage": {
    "Provider": "Local",
    "Local": {
      "Path": "E:\\files"
    },
  },
  ```

- Use Azure Blob:
  ```js
  "Storage": {
    "Provider": "Azure",
    "Azure": {
      "ConnectionString": "xxx",
      "Container": "classifiedadds"
    },
  },
  ```

- Use Amazon S3:
  ```js
  "Storage": {
    "Provider": "Amazon",
    "Amazon": {
      "AccessKeyID": "xxx",
      "SecretAccessKey": "xxx",
      "BucketName": "classifiedadds",
      "RegionEndpoint": "ap-southeast-1"
    }
  },
  ```

## Configure Message Broker

- Open [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebMVC/appsettings.json) and [ClassifiedAds.BackgroundServer/appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.BackgroundServer/appsettings.json) and jump to **MessageBroker** section.
  ```js
  "MessageBroker": {
    "Provider": "RabbitMQ",
  }
  ```

- Use RabbitMQ
  ```js
  "MessageBroker": {
    "Provider": "RabbitMQ",
    "RabbitMQ": {
      "HostName": "localhost",
      "UserName": "guest",
      "Password": "guest",
      "ExchangeName": "amq.direct",
      "RoutingKey_FileUploaded": "classifiedadds_fileuploaded",
      "RoutingKey_FileDeleted": "classifiedadds_filedeleted",
      "QueueName_FileUploaded": "classifiedadds_fileuploaded",
      "QueueName_FileDeleted": "classifiedadds_filedeleted"
    },
  }
  ```

- Use Kafka:
  ```js
  "MessageBroker": {
    "Provider": "Kafka",
    "Kafka": {
      "BootstrapServers": "localhost:9092",
      "Topic_FileUploaded": "classifiedadds_fileuploaded",
      "Topic_FileDeleted": "classifiedadds_filedeleted"
    },
  }
  ```

- Use Azure Queue Storage:
  ```js
  "MessageBroker": {
    "Provider": "AzureQueue",
    "AzureQueue": {
      "ConnectionString": "xxx",
      "QueueName_FileUploaded": "classifiedadds-fileuploaded",
      "QueueName_FileDeleted": "classifiedadds-filedeleted"
    },
  }
  ```

- Use Azure Service Bus:
  ```js
  "MessageBroker": {
    "Provider": "AzureServiceBus",
    "AzureServiceBus": {
      "ConnectionString": "xxx",
      "QueueName_FileUploaded": "classifiedadds_fileuploaded",
      "QueueName_FileDeleted": "classifiedadds_filedeleted"
    }
  }
  ```

## Set Startup Projects
![alt text](/docs/imgs/startup-projects.png)

## Run or Debug the Solution
- Web MVC Home Page: https://localhost:44364/

  ![alt text](/docs/imgs/web-mvc-home-page.png)

- Navigate to Health Checks UI https://localhost:44364/healthchecks-ui#/healthchecks and make sure everything is green.

  ![alt text](/docs/imgs/health-checks-ui.png)

- Login on Identity Server:
  + Option 1: Use default created account:
    + User Name: phong@gmail.com
    + Password: v*7Un8b4rcN@<-RN
  + Option 2: Register new account at https://localhost:44367/Account/Register
  
  ![alt text](/docs/imgs/identity-server-login-page.png)

- Open Blazor Home Page at: https://localhost:44331

  ![alt text](/docs/imgs/blazor-home-page.png)

## How to Build and Run Single Page Applications:
- Angular:
  + Navigate to folder: [ClassifiedAds.UIs/angular/](/src/ClassifiedAds.UIs/angular/)
    ```
    npm install
    ng serve
    ```
  + Go to http://localhost:4200/

    ![alt text](/docs/imgs/angular-home-page.png)
  
- React:
  + Navigate to folder: [ClassifiedAds.UIs/reactjs/](/src/ClassifiedAds.UIs/reactjs/)
    ```
    npm install
    npm run start
    ```
  + Go to http://localhost:3000/
  
    ![alt text](/docs/imgs/react-home-page.png)
  
- Vue:
  + Navigate to folder: [ClassifiedAds.UIs/vuejs/](/src/ClassifiedAds.UIs/vuejs/)
    ```
    npm install
    npm run serve
    ```
  + Go to http://localhost:8080/
  
    ![alt text](/docs/imgs/vue-home-page.png)

- Before Login, go to Identity Server https://localhost:44367/Client to make sure application clients have been registered:

  ![alt text](/docs/imgs/identity-server-clients-page.png)
    
## How to Run on Docker Containers:
- Add Migrations if you haven't done on previous steps:
  + Install **dotnet-ef** cli:
    ```
    dotnet tool install --global dotnet-ef --version="3.1"
    ```
  + Navigate to [ClassifiedAds.Migrator](/src/ClassifiedAds.Projects/ClassifiedAds.Migrator/) and run these commands:
    ```
    dotnet ef migrations add Init --context AdsDbContext -o Migrations/AdsDb
    dotnet ef migrations add Init --context ConfigurationDbContext -o Migrations/ConfigurationDb
    dotnet ef migrations add Init --context PersistedGrantDbContext -o Migrations/PersistedGrantDb
    ```
- Navigate to [ClassifiedAds.Projects](/src/ClassifiedAds.Projects/) and run:
  ```
  docker-compose build
  docker-compose up
  ```
- Open Web MVC Home Page at: http://host.docker.internal:9003

  ![alt text](/docs/imgs/web-mvc-home-page.png)

- Navigate to Health Checks UI http://host.docker.internal:9003/healthchecks-ui#/healthchecks and make sure everything is green.

  ![alt text](/docs/imgs/health-checks-ui-container.png)

- Login on Identity Server:
  + Use default created account: phong@gmail.com / v*7Un8b4rcN@<-RN
  + Register new account at http://host.docker.internal:9000/Account/Register
  
- Open Blazor Home Page at: http://host.docker.internal:9008

  ![alt text](/docs/imgs/blazor-home-page.png)
  
## Application URLs:
| Project  | Launch URL | Docker Container URL| Docker Container URL|
| -------- | ---------- | ------------------- | ------------------- |
| BackgroundServer | https://localhost:44318 | http://localhost:9004 | http://host.docker.internal:9004 |
| Blazor | https://localhost:44331 | http://localhost:9008 | http://host.docker.internal:9008 |
| GRPC | https://localhost:5001 | https://localhost:9005 | https://host.docker.internal:9005 |
| IdentityServer | https://localhost:44367 | http://localhost:9000 | http://host.docker.internal:9000 |
| NotificationServer | https://localhost:44390 | http://localhost:9001 | http://host.docker.internal:9001 |
| WebAPI | https://localhost:44312 | http://localhost:9002 | http://host.docker.internal:9002 |
| WebMVC | https://localhost:44364 | http://localhost:9003 | http://host.docker.internal:9003 |
| GraphQL | [https://localhost:44392](https://localhost:44392/ui/playground) | [http://localhost:9006](http://localhost:9006/ui/playground) | [http://host.docker.internal:9006](http://host.docker.internal:9006/ui/playground) |
| Ocelot | [https://localhost:44340](https://localhost:44340/ocelot/products) | [http://localhost:9007](http://localhost:9007/ocelot/products) | [http://host.docker.internal:9007](http://host.docker.internal:9007/ocelot/products) |
