[Domain-Driven Design Path | Pluralsight](https://app.pluralsight.com/paths/skills/domain-driven-design)

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
## Update Configuration
<details>
  <summary><b>Database</b></summary>
  
- Update Connection Strings:

  | Project  | Configuration File | Configuration Key |
  | -------- | ------------------ | ----------------- |
  | ClassifiedAds.Migrator | [appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.Migrator/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.BackgroundServer | [appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.BackgroundServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.IdentityServer | [appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.IdentityServer/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.WebAPI | [appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json) | ConnectionStrings:ClassifiedAds |
  | ClassifiedAds.WebMVC | [appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json) | ConnectionStrings:ClassifiedAds |


- Run Migration:
  + Option 1: Using dotnet cli:
    + Install **dotnet-ef** cli:
      ```
      dotnet tool install --global dotnet-ef --version="3.1"
      ```
    + Navigate to [ClassifiedAds.Migrator](/src/ClassifiedAds.Monolith/ClassifiedAds.Migrator/) and run these commands:
      ```
      dotnet ef migrations add Init --context AdsDbContext -o Migrations/AdsDb
      dotnet ef migrations add Init --context ConfigurationDbContext -o Migrations/ConfigurationDb
      dotnet ef migrations add Init --context PersistedGrantDbContext -o Migrations/PersistedGrantDb
      dotnet ef migrations add Init --context MiniProfilerDbContext -o Migrations/MiniProfilerDb
      dotnet ef database update --context AdsDbContext
      dotnet ef database update --context ConfigurationDbContext
      dotnet ef database update --context PersistedGrantDbContext
      dotnet ef database update --context MiniProfilerDbContext
      ```
  + Option 2: Using Package Manager Console:
    + Set **ClassifiedAds.Migrator** as StartUp Project
    + Open Package Manager Console, select **ClassifiedAds.Migrator** as Default Project
    + Run these commands:
      ```
      Add-Migration -Context AdsDbContext Init -OutputDir Migrations/AdsDb
      Add-Migration -Context ConfigurationDbContext Init -OutputDir Migrations/ConfigurationDb
      Add-Migration -Context PersistedGrantDbContext Init -OutputDir Migrations/PersistedGrantDb
      Add-Migration -Context MiniProfilerDbContext Init -OutputDir Migrations/MiniProfilerDb
      Update-Database -Context AdsDbContext
      Update-Database -Context ConfigurationDbContext
      Update-Database -Context PersistedGrantDbContext
      Update-Database -Context MiniProfilerDbContext
      ```
</details>

<details>
  <summary><b>Additional Configuration Sources</b></summary>
  
  - Open [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json) and jump to **ConfigurationSources** section.
    ```js
    "ConfigurationSources": {
      "SqlServer": {
        "IsEnabled": false,
        "ConnectionString": "Server=.;Database=ClassifiedAds;User Id=sa;Password=sqladmin123!@#",
        "SqlQuery": "select [Key], [Value] from ConfigurationEntries"
      },
      "AzureKeyVault": {
        "IsEnabled": false,
        "VaultName": "https://xxx.vault.azure.net/"
      }
    },
    ```

  - Get from Sql Server database:
    ```js
    "ConfigurationSources": {
      "SqlServer": {
        "IsEnabled": true,
        "ConnectionString": "Server=.;Database=ClassifiedAds;User Id=sa;Password=sqladmin123!@#",
        "SqlQuery": "select [Key], [Value] from ConfigurationEntries"
      },
    },
    ```

  - Get from Azure Key Vault:
    ```js
    "ConfigurationSources": {
      "AzureKeyVault": {
        "IsEnabled": true,
        "VaultName": "https://xxx.vault.azure.net/"
      }
    },
    ```

  - Use Both:
    ```js
    "ConfigurationSources": {
      "SqlServer": {
        "IsEnabled": true,
        "ConnectionString": "Server=.;Database=ClassifiedAds;User Id=sa;Password=sqladmin123!@#",
        "SqlQuery": "select [Key], [Value] from ConfigurationEntries"
      },
      "AzureKeyVault": {
        "IsEnabled": true,
        "VaultName": "https://xxx.vault.azure.net/"
      }
    },
    ```
</details>

<details>
  <summary><b>Storage</b></summary>
  
  - Open [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json), [ClassifiedAds.WebAPI/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json) and jump to **Storage** section.
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
</details>

<details>
  <summary><b>Message Broker</b></summary>
  
  - Open [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json), [ClassifiedAds.WebAPI/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json), [ClassifiedAds.BackgroundServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.BackgroundServer/appsettings.json) and jump to **MessageBroker** section.
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
      }
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
      }
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
      }
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
	
  - Use Azure Event Grid:
    ```js
    "MessageBroker": {
      "Provider": "AzureEventGrid",
      "AzureEventGrid": {
        "DomainEndpoint": "https://xxx.xxx-1.eventgrid.azure.net/api/events",
        "DomainKey": "xxxx",
        "Topic_FileUploaded": "classifiedadds_fileuploaded",
        "Topic_FileDeleted": "classifiedadds_filedeleted"
      }
    }
    ```
	
  - Use Azure Event Hubs:
    ```js
    "MessageBroker": {
      "Provider": "AzureEventHub",
      "AzureEventHub": {
        "ConnectionString": "Endpoint=sb://xxx.servicebus.windows.net/;SharedAccessKeyName=xxx;SharedAccessKey=xxx",
        "Hub_FileUploaded": "classifiedadds_fileuploaded",
        "Hub_FileDeleted": "classifiedadds_filedeleted",
        "StorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=xxx;AccountKey=xxx;EndpointSuffix=core.windows.net",
        "StorageContainerName_FileUploaded": "eventhub-fileuploaded",
        "StorageContainerName_FileDeleted": "eventhub-filedeleted"
      }
    }
    ```
</details>

<details>
  <summary><b>Logging</b></summary>
  
  - Open and jump to **Logging** section of below files:
    + [ClassifiedAds.WebAPI/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json)
    + [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json)
    + [ClassifiedAds.IdentityServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.IdentityServer/appsettings.json)
    + [ClassifiedAds.BackgroundServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.BackgroundServer/appsettings.json)
    ```js
    "Logging": {
      "LogLevel": {
        "Default": "Warning"
      },
      "File": {
        "MinimumLogEventLevel": "Information"
      },
      "Elasticsearch": {
        "IsEnabled": false,
        "Host": "http://localhost:9200",
        "IndexFormat": "classifiedads",
        "MinimumLogEventLevel": "Information"
      },
      "EventLog": {
        "IsEnabled": false,
        "LogName": "Application",
        "SourceName": "ClassifiedAds.WebAPI"
      }
    },
    ```
  - Write to Local file (./logs/log.txt). Always enabled.
    ```js
    "Logging": {
      "File": {
        "MinimumLogEventLevel": "Information"
      },
    },
    ```
  - Write to Elasticsearch:
    ```js
    "Logging": {
      "Elasticsearch": {
        "IsEnabled": true,
        "Host": "http://localhost:9200",
        "IndexFormat": "classifiedads",
        "MinimumLogEventLevel": "Information"
      },
    },
    ```
  - Write to Windows Event Log (Windows only):
    ```js
    "Logging": {
      "EventLog": {
        "IsEnabled": true,
        "LogName": "Application",
        "SourceName": "ClassifiedAds.WebAPI"
      }
    },
    ```
  - Enable all options:
    ```js
    "Logging": {
      "LogLevel": {
        "Default": "Warning"
      },	
      "File": {
        "MinimumLogEventLevel": "Information"
      },
      "Elasticsearch": {
        "IsEnabled": true,
        "Host": "http://localhost:9200",
        "IndexFormat": "classifiedads",
        "MinimumLogEventLevel": "Information"
      },
      "EventLog": {
        "IsEnabled": true,
        "LogName": "Application",
        "SourceName": "ClassifiedAds.WebAPI"
      }
    },
    ```
</details>

<details>
  <summary><b>Caching</b></summary>
  
  - Open and jump to **Caching** section of below files:
    + [ClassifiedAds.WebAPI/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json)
    + [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json)
    + [ClassifiedAds.IdentityServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.IdentityServer/appsettings.json)
    ```js
    "Caching": {
      "InMemory": {

      },
      "Distributed": {

      }
    },
    ```
  - Configure options for In Memory Cache:
    ```js
    "Caching": {
      "InMemory": {
        "SizeLimit": null
      },
    },
    ```
  - Use In Memory Distributed Cache (For Local Testing):
    ```js
    "Caching": {
      "Distributed": {
        "Provider": "InMemory",
        "InMemory": {
          "SizeLimit": null
        }
      }
    },
    ```
  - Use Redis Distributed Cache:
    ```js
    "Caching": {
      "Distributed": {
        "Provider": "Redis",
        "Redis": {
          "Configuration": "xxx.redis.cache.windows.net:6380,password=xxx,ssl=True,abortConnect=False",
          "InstanceName": ""
        }
      }
    },
    ```
  - Use Sql Server Distributed Cache:
    ```js
	dotnet tool install --global dotnet-sql-cache --version="3.1"
	dotnet sql-cache create "Server=.;Database=ClassifiedAds;User Id=sa;Password=sqladmin123!@#" dbo CacheEntries
    ```
    ```js
    "Caching": {
      "Distributed": {
        "Provider": "SqlServer",
        "SqlServer": {
          "ConnectionString": "Server=.;Database=ClassifiedAds;User Id=sa;Password=sqladmin123!@#",
          "SchemaName": "dbo",
          "TableName": "CacheEntries"
        }
      }
    },
    ```
</details>

<details>
  <summary><b>Monitoring</b></summary>
  
  - Open and jump to **Monitoring** section of below files:
    + [ClassifiedAds.WebAPI/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json)
    + [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json)
    + [ClassifiedAds.IdentityServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.IdentityServer/appsettings.json)
    ```js
    "Monitoring": {
      "MiniProfiler": {
        
      },
      "AzureApplicationInsights": {
        
      }
    },
    ```
  - Use MiniProfiler:
    ```js
    "Monitoring": {
      "MiniProfiler": {
        "IsEnabled": true,
        "SqlServerStorage": {
          "ConectionString": "Server=.;Database=ClassifiedAds;User Id=sa;Password=sqladmin123!@#;MultipleActiveResultSets=true",
          "ProfilersTable": "MiniProfilers",
          "TimingsTable": "MiniProfilerTimings",
          "ClientTimingsTable": "MiniProfilerClientTimings"
        }
      },
    },
    ```
  - Use Azure Application Insights:
    ```js
	"Monitoring": {
      "AzureApplicationInsights": {
        "IsEnabled": true,
		"InstrumentationKey": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
		"EnableSqlCommandTextInstrumentation": true
      }
	},
    ```
  - Use Both:
    ```js
    "Monitoring": {
      "MiniProfiler": {
        "IsEnabled": true,
        "SqlServerStorage": {
          "ConectionString": "Server=.;Database=ClassifiedAds;User Id=sa;Password=sqladmin123!@#;MultipleActiveResultSets=true",
          "ProfilersTable": "MiniProfilers",
          "TimingsTable": "MiniProfilerTimings",
          "ClientTimingsTable": "MiniProfilerClientTimings"
        }
      },
      "AzureApplicationInsights": {
        "IsEnabled": true,
        "InstrumentationKey": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
        "EnableSqlCommandTextInstrumentation": true
      }
    },
    ```
</details>

<details>
  <summary><b>Interceptors</b></summary>
  
  - Open and jump to **Interceptors** section of below files:
    + [ClassifiedAds.WebAPI/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json)
    + [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json)
    + [ClassifiedAds.IdentityServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.IdentityServer/appsettings.json)
    + [ClassifiedAds.BackgroundServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.BackgroundServer/appsettings.json)
    ```js
    "Interceptors": {
      "LoggingInterceptor": true,
      "ErrorCatchingInterceptor": false
    },
    ```
</details>

<details>
  <summary><b>Security Headers</b></summary>
  
  - Open [ClassifiedAds.WebAPI/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json) and jump to **SecurityHeaders** section:
    ```js
    "SecurityHeaders": {
      "Cache-Control": "no-cache, no-store, must-revalidate",
      "Pragma": "no-cache",
      "Expires": "0"
    },
    ```
  - Open [ClassifiedAds.WebMVC/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebMVC/appsettings.json) and jump to **SecurityHeaders** section:
    ```js
    "SecurityHeaders": {
      "Content-Security-Policy": "form-action 'self'; frame-ancestors 'none'",
      "Feature-Policy": "camera 'none'",
      "Referrer-Policy": "strict-origin-when-cross-origin",
      "X-Content-Type-Options": "nosniff",
      "X-Frame-Options": "DENY",
      "X-XSS-Protection": "1; mode=block",
      "Cache-Control": "no-cache, no-store, must-revalidate",
      "Pragma": "no-cache",
      "Expires": "0"
    },
    ```
</details>

<details>
  <summary><b>Cross-Origin Resource Sharing (CORS)</b></summary>
  
  - Open [ClassifiedAds.WebAPI/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.WebAPI/appsettings.json) and jump to **CORS** section:
    ```js
    "CORS": {
      "AllowAnyOrigin": false,
      "AllowedOrigins": [ "http://localhost:4200", "http://localhost:3000", "http://localhost:8080" ]
    },
    ```
  - Open [ClassifiedAds.NotificationServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.NotificationServer/appsettings.json) and jump to **CORS** section:
    ```js
    "CORS": {
      "AllowedOrigins": [ "https://localhost:44364", "http://host.docker.internal:9003" ]
    }
    ```
</details>

<details>
  <summary><b>External Login</b></summary>
  
  - Open [ClassifiedAds.IdentityServer/appsettings.json](/src/ClassifiedAds.Monolith/ClassifiedAds.IdentityServer/appsettings.json) and jump to **ExternalLogin** section:
    ```js
    "ExternalLogin": {
      "AzureActiveDirectory": {
        "IsEnabled": true,
        "Authority": "https://login.microsoftonline.com/<Directory (tenant) ID>",
        "ClientId": "<Application (client) ID",
        "ClientSecret": "xxx"
      },
      "Microsoft": {
        "IsEnabled": true,
        "ClientId": "<Application (client) ID",
        "ClientSecret": "xxx"
      },
      "Google": {
        "IsEnabled": true,
        "ClientId": "xxx",
        "ClientSecret": "xxx"
      },
      "Facebook": {
        "IsEnabled": true,
        "AppId": "xxx",
        "AppSecret": "xxx"
      }
    },
    ```
</details>

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
  + Navigate to [ClassifiedAds.Migrator](/src/ClassifiedAds.Monolith/ClassifiedAds.Migrator/) and run these commands:
    ```
    dotnet ef migrations add Init --context AdsDbContext -o Migrations/AdsDb
    dotnet ef migrations add Init --context ConfigurationDbContext -o Migrations/ConfigurationDb
    dotnet ef migrations add Init --context PersistedGrantDbContext -o Migrations/PersistedGrantDb
    dotnet ef migrations add Init --context MiniProfilerDbContext -o Migrations/MiniProfilerDb
    ```
- Navigate to [ClassifiedAds.Monolith](/src/ClassifiedAds.Monolith/) and run:
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
| IdentityServer | https://localhost:44367 | http://localhost:9000 | http://host.docker.internal:9000 |
| NotificationServer | https://localhost:44390 | http://localhost:9001 | http://host.docker.internal:9001 |
| WebAPI | https://localhost:44312 | http://localhost:9002 | http://host.docker.internal:9002 |
| WebMVC | https://localhost:44364 | http://localhost:9003 | http://host.docker.internal:9003 |
| GraphQL | [https://localhost:44392](https://localhost:44392/ui/playground) | [http://localhost:9006](http://localhost:9006/ui/playground) | [http://host.docker.internal:9006](http://host.docker.internal:9006/ui/playground) |
| Ocelot | [https://localhost:44340](https://localhost:44340/ocelot/products) | [http://localhost:9007](http://localhost:9007/ocelot/products) | [http://host.docker.internal:9007](http://host.docker.internal:9007/ocelot/products) |
| Angular | http://localhost:4200/ | | |
| React | http://localhost:3000/ | | |
| Vue | http://localhost:8080/ | | |
