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
  | ClassifiedAds.BackgroundServices | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.BackgroundServices/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.GRPC | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.GRPC/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.IdentityServer | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.IdentityServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.NotificationServer | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.NotificationServer/appsettings.json) | |
  | ClassifiedAds.WebAPI | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebAPI/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.WebMVC | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebMVC/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.GraphQL | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.GraphQL/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.Ocelot | [appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.Ocelot/appsettings.json) |  |


- Run Migration:
  + Option 1: Using dotnet cli:
    + Install **dotnet-ef** cli:
      ```
      dotnet tool install --global dotnet-ef --version="3.1"
      ```
    + Navigate to [ClassifiedAds.Migrator](/src/ClassifiedAds.Projects/ClassifiedAds.Migrator/) and run these commands:
      ```
      dotnet ef migrations add Init --context AdsDbContext
      dotnet ef database update --context AdsDbContext
      dotnet ef database update --context ConfigurationDbContext
      dotnet ef database update --context PersistedGrantDbContext
      ```
  + Option 2: Using Package Manager Console:
    + Set **ClassifiedAds.Migrator** as StartUp Project
    + Open Package Manager Console, select **ClassifiedAds.Migrator** as Default Project
    + Run these commands:
      ```
      Add-Migration -Context AdsDbContext Init
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

- Open [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Projects/ClassifiedAds.WebMVC/appsettings.json) and jump to **MessageBroker** section.
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
      "RoutingKey_FileDeleted": "classifiedadds_filedeleted"
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

## How to Login on Identity Server:
- Option 1: Use default created account:
  + User Name: phong@gmail.com
  + Password: v*7Un8b4rcN@<-RN
- Option 2: Register new account at https://localhost:44367/Account/Register
