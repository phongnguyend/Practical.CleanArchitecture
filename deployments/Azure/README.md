- Login Azure
```
az login
```

- Create Resource Group
```
az group create --name "ClassifiedAds_DEV" \
                --location "southeastasia" \
                --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD" "ResourceType=Mixed"
```

- Create Storage Account
```
az storage account create --resource-group "ClassifiedAds_DEV" \
                          --name "classifiedadsdev" \
                          --location "southeastasia" \
                          --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Create SQL Server (classifiedads/sqladmin123!@#)
```
az sql server create --resource-group "ClassifiedAds_DEV" \
                     --name "classifiedadsdev" \
                     --location "southeastasia" \
                     --admin-user "classifiedads" \
                     --admin-password 'sqladmin123!@#'
```

- Configure Firewall
```
az sql server firewall-rule create --resource-group "ClassifiedAds_DEV" \
                                   --server "classifiedadsdev" \
                                   --name "Allow Azure services and resources to access this server" \
                                   --start-ip-address 0.0.0.0 \
                                   --end-ip-address 0.0.0.0
                                    
 az sql server firewall-rule create --resource-group "ClassifiedAds_DEV" \
                                    --server "classifiedadsdev" \
                                    --name "Your IP Address" \
                                    --start-ip-address "Your IP Address" \
                                    --end-ip-address "Your IP Address"                                   
```

- Create SQL Database
```
az sql db create --resource-group "ClassifiedAds_DEV" \
                 --name "classifiedadsdevdb" \
                 --server  "classifiedadsdev" \
                 --service-objective Basic \
                 --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Create Key Vault
```
az keyvault create --resource-group "ClassifiedAds_DEV" \
                   --name "classifiedadsdev" \
                   --location "southeastasia" \
                   --sku Standard \
                   --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Creeate Service Bus Name Space
```
az servicebus namespace create --resource-group "ClassifiedAds_DEV" \
                               --name classifiedadsdev \
                               --location "southeastasia" \
                               --sku Standard \
                               --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Create Service Bus Queues
```
az servicebus queue create --resource-group "ClassifiedAds_DEV" \
                           --namespace-name "classifiedadsdev" \
                           --name classifiedadds_fileuploaded \
                           --max-size 1024
                           
az servicebus queue create --resource-group "ClassifiedAds_DEV" \
                           --namespace-name "classifiedadsdev" \
                           --name classifiedadds_filedeleted \
                           --max-size 1024
```

- Create Service Bus Topics
```
az servicebus topic create --resource-group "ClassifiedAds_DEV" \
                           --namespace-name "classifiedadsdev" \
                           --name topic_fileuploaded \
                           --max-size 1024
                            
 az servicebus topic create --resource-group "ClassifiedAds_DEV" \
                            --namespace-name "classifiedadsdev" \
                            --name topic_filedeleted \
                            --max-size 1024
```

- Create Service Bus Subscriptions
```
az servicebus topic subscription create --resource-group "ClassifiedAds_DEV" \
                                        --namespace-name "classifiedadsdev" \
                                        --topic-name topic_fileuploaded \
                                        --name sub_fileuploaded
                            
az servicebus topic subscription create --resource-group "ClassifiedAds_DEV" \
                                        --namespace-name "classifiedadsdev" \
                                        --topic-name topic_filedeleted \
                                        --name sub_filedeleted
```

- Create Event Grid Domain
```
az eventgrid domain create --resource-group "ClassifiedAds_DEV" \
                           --name classifiedadsdev \
                           --location "southeastasia" \
                           --sku Basic \
                           --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Create Event Hub Name Space
```
az eventhubs namespace create --resource-group "ClassifiedAds_DEV" \
                              --name classifiedadshubsdev \
                              --location "southeastasia" \
                              --sku Basic \
                              --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Create Event Hubs
```
az eventhubs eventhub create --resource-group "ClassifiedAds_DEV" \
                             --namespace-name "classifiedadshubsdev" \
                             --name classifiedadds_fileuploaded \
                             --message-retention 1 \
                             --partition-count 2
                             
az eventhubs eventhub create --resource-group "ClassifiedAds_DEV" \
                             --namespace-name "classifiedadshubsdev" \
                             --name classifiedadds_filedeleted \
                             --message-retention 1 \
                             --partition-count 2
```

- Create Application Insights
```
az extension add -n application-insights

az monitor app-insights component create --resource-group "ClassifiedAds_DEV" \
                                         --app ClassifiedAds.WebAPI \
                                         --location "southeastasia" \
                                         --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"

az monitor app-insights component create --resource-group "ClassifiedAds_DEV" \
                                         --app ClassifiedAds.WebMVC \
                                         --location "southeastasia" \
                                         --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"

az monitor app-insights component create --resource-group "ClassifiedAds_DEV" \
                                         --app ClassifiedAds.IdentityServer \
                                         --location "southeastasia" \
                                         --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Create Redis Cache
```
az redis create --location "southeastasia" \
                --name classifiedads \
                --resource-group "ClassifiedAds_DEV" \
                --sku Standard \
                --vm-size c0 \
                --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
                
az redis list-keys --resource-group ClassifiedAds_DEV \
                    --name classifiedads               
```

- Create Azure Container Registry
```
az acr create --resource-group "ClassifiedAds_DEV" \
              --name classifiedads \
              --location "southeastasia" \
              --sku Basic \
              --admin-enabled true \
              --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Create App Service Plan
```
az appservice plan create --resource-group "ClassifiedAds_DEV" \
                          --name ClassifiedAds-Hosts \
                          --location "southeastasia" \
                          --sku B1 \
                          --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
```

- Create App Services:
```
az webapp create --resource-group "ClassifiedAds_DEV" \
                 --plan ClassifiedAds-Hosts \
                 --name identityserver-classifiedads \
                 --runtime "DOTNETCORE|3.1" \
                 --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"

az webapp create --resource-group "ClassifiedAds_DEV" \
                 --plan ClassifiedAds-Hosts \
                 --name webapi-classifiedads \
                 --runtime "DOTNETCORE|3.1" \
                 --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"
                 
az webapp create --resource-group "ClassifiedAds_DEV" \
                 --plan ClassifiedAds-Hosts \
                 --name webmvc-classifiedads \
                 --runtime "DOTNETCORE|3.1" \
                 --tags "Environment=Development" "Project=ClassifiedAds" "Department=SD"                 
```

- Configure Connection String
```
connectionString=$(az sql db show-connection-string -s classifiedadsdev -n classifiedadsdevdb -c ado.net --out tsv)
connectionString=${connectionString/<username>/classifiedads}
connectionString=${connectionString/<password>/'sqladmin123!@#'}

echo $connectionString

## IdentityServer
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name identityserver-classifiedads \
  --settings "ConnectionStrings__ClassifiedAds=$connectionString"

## WebAPI 
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webapi-classifiedads \
  --settings "ConnectionStrings__ClassifiedAds=$connectionString"

## WebMVC
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webmvc-classifiedads \
  --settings "ConnectionStrings__ClassifiedAds=$connectionString"
```

- Configure Storage
```
storageConnectionString=$(az storage account show-connection-string  \
  --resource-group ClassifiedAds_DEV  \
  --name classifiedadsdev  \
  --key primary  \
  --query connectionString  \
  --out tsv)
  
echo $storageConnectionString

## WebAPI
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webapi-classifiedads \
  --settings "Storage__Provider=Azure" \
             "Storage__Azure_ConnectionString=$storageConnectionString"

## WebMVC
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webmvc-classifiedads \
  --settings "Storage__Provider=Azure" \
             "Storage__Azure_ConnectionString=$storageConnectionString"
```

- Configure MessageBroker
```
storageConnectionString=$(az storage account show-connection-string  \
  --resource-group ClassifiedAds_DEV  \
  --name classifiedadsdev  \
  --key primary  \
  --query connectionString  \
  --out tsv)
  
echo $storageConnectionString

## WebAPI
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webapi-classifiedads \
  --settings "MessageBroker__Provider=AzureQueue" \
             "MessageBroker__AzureQueue_ConnectionString=$storageConnectionString"

## WebMVC
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webmvc-classifiedads \
  --settings "MessageBroker__Provider=AzureQueue" \
             "MessageBroker__AzureQueue_ConnectionString=$storageConnectionString"
```

- Configure Distributed Cache
```
redisKey=$(az redis list-keys --resource-group ClassifiedAds_DEV --name classifiedads --query primaryKey --out tsv)
redisConnectionString='classifiedads.redis.cache.windows.net:6380,password='$redisKey',ssl=True,abortConnect=False'
echo $redisConnectionString

## IdentityServer
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name identityserver-classifiedads \
  --settings "Caching__Distributed__Provider=Redis" \
             "Caching__Distributed__Redis__Configuration=$redisConnectionString"

## WebAPI
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webapi-classifiedads \
  --settings "Caching__Distributed__Provider=Redis" \
             "Caching__Distributed__Redis__Configuration=$redisConnectionString"

## WebMVC
az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webmvc-classifiedads \
  --settings "Caching__Distributed__Provider=Redis" \
             "Caching__Distributed__Redis__Configuration=$redisConnectionString"
```

- Configure Monitoring
```
## IdentityServer
instrumentationKey=$(az monitor app-insights component show \
  --resource-group ClassifiedAds_DEV  \
  --app ClassifiedAds.IdentityServer  \
  --query instrumentationKey  \
  --out tsv)
  
echo $instrumentationKey

az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name identityserver-classifiedads \
  --settings "Monitoring__AzureApplicationInsights__IsEnabled=true" \
             "Monitoring__AzureApplicationInsights__InstrumentationKey=$instrumentationKey"

## WebAPI
instrumentationKey=$(az monitor app-insights component show \
  --resource-group ClassifiedAds_DEV  \
  --app ClassifiedAds.WebAPI  \
  --query instrumentationKey  \
  --out tsv)
  
echo $instrumentationKey

az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webapi-classifiedads \
  --settings "Monitoring__MiniProfiler__IsEnabled=false" \
             "Monitoring__AzureApplicationInsights__IsEnabled=true" \
             "Monitoring__AzureApplicationInsights__InstrumentationKey=$instrumentationKey"

## WebMVC
instrumentationKey=$(az monitor app-insights component show \
  --resource-group ClassifiedAds_DEV  \
  --app ClassifiedAds.WebMVC  \
  --query instrumentationKey  \
  --out tsv)
  
echo $instrumentationKey

az webapp config appsettings set \
  --resource-group ClassifiedAds_DEV \
  --name webmvc-classifiedads \
  --settings "Monitoring__AzureApplicationInsights__IsEnabled=true" \
             "Monitoring__AzureApplicationInsights__InstrumentationKey=$instrumentationKey"
```

- Create Lock
```
az lock create --lock-type CanNotDelete \
               --name CanNotDelete \
               --resource-group ClassifiedAds_DEV
```

- Clean Up
```
az lock delete --name CanNotDelete --resource-group ClassifiedAds_DEV
az group delete --name "ClassifiedAds_DEV" --yes
```
