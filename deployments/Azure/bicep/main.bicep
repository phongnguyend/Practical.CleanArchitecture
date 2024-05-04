var storageAccountName = 'classifiedadsdev'
var sqlServerName = 'classifiedadsdev'
var sqlServerUserName = 'classifiedads'
var sqlServerPassword = 'sqladmin123!@#'
var tenantId = subscription().tenantId

@description('The location in which the Azure Storage resources should be deployed.')
param location string = resourceGroup().location

@description('Specifies the object ID of a user, service principal or security group in the Azure Active Directory tenant for the vault. The object ID must be unique for the list of access policies. Get it by using Get-AzADUser or Get-AzADServicePrincipal cmdlets.')
param objectId string = 'cea61931-f7a9-4552-acc2-7958313b02fd'

resource storageaccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
  tags: {
    Environment: 'Development'
    Project: 'ClassifiedAds'
    Department: 'SD'
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlServerUserName
    administratorLoginPassword: sqlServerPassword
  }
  tags: {
    Environment: 'Development'
    Project: 'ClassifiedAds'
    Department: 'SD'
  }
}

resource firewallRule1 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  name: 'Allow Azure services and resources to access this server'
  parent: sqlServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource firewallRule2 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  name: 'Your IP Address'
  parent: sqlServer
  properties: {
    startIpAddress: '171.243.49.233'
    endIpAddress: '171.243.49.233'
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: 'classifiedadsdevdb'
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  tags: {
    Environment: 'Development'
    Project: 'ClassifiedAds'
    Department: 'SD'
  }
}

resource keyvault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: 'classifiedadsdev'
  location: location
  properties: {
    tenantId: tenantId
    accessPolicies: [
      {
        objectId: objectId
        tenantId: tenantId
        permissions: {
          keys: ['list']
          secrets: ['list']
        }
      }
    ]
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
  tags: {
    Environment: 'Development'
    Project: 'ClassifiedAds'
    Department: 'SD'
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: 'ClassifiedAds-Hosts'
  location: location
  sku: {
    name: 'B1'
  }
  tags: {
    Environment: 'Development'
    Project: 'ClassifiedAds'
    Department: 'SD'
  }
}

var sqlServerConnectionString = 'Server=tcp:${sqlServer.name}.${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${sqlDb.name};Persist Security Info=False;User ID=${sqlServerUserName};Password=${sqlServerPassword};MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;'

resource appService1 'Microsoft.Web/sites@2023-01-01' = {
  name: 'identityserver-classifiedads'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      windowsFxVersion: 'DOTNET|8.0'
      appSettings: [
        {
          name: 'ConnectionStrings__ClassifiedAds'
          value: sqlServerConnectionString
        }
      ]
    }
  }
  tags: {
    Environment: 'Development'
    Project: 'ClassifiedAds'
    Department: 'SD'
  }
}

resource appService2 'Microsoft.Web/sites@2023-01-01' = {
  name: 'webapi-classifiedads'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      windowsFxVersion: 'DOTNET|8.0'
      appSettings: [
        {
          name: 'ConnectionStrings__ClassifiedAds'
          value: sqlServerConnectionString
        }
      ]
    }
  }
  tags: {
    Environment: 'Development'
    Project: 'ClassifiedAds'
    Department: 'SD'
  }
}

resource appService3 'Microsoft.Web/sites@2023-01-01' = {
  name: 'webmvc-classifiedads'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      windowsFxVersion: 'DOTNET|8.0'
      appSettings: [
        {
          name: 'ConnectionStrings__ClassifiedAds'
          value: sqlServerConnectionString
        }
      ]
    }
  }
  tags: {
    Environment: 'Development'
    Project: 'ClassifiedAds'
    Department: 'SD'
  }
}

output tenantId string = tenantId
