provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "ClassifiedAds_DEV"
  location = "Southeast Asia"

  tags = local.common_tags
}

resource "azurerm_storage_account" "sa" {
  name                     = var.storageAccountName
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = local.common_tags
}

resource "azurerm_mssql_server" "sqlserver" {
  name                         = var.sqlServerName
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sqlServerUserName
  administrator_login_password = var.sqlServerPassword

  tags = local.common_tags
}

resource "azurerm_mssql_firewall_rule" "FirewallRule1" {
  name             = "Allow Azure services and resources to access this server"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_mssql_firewall_rule" "FirewallRule2" {
  name             = "Your IP Address"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "171.243.49.233"
  end_ip_address   = "171.243.49.233"
}

resource "azurerm_mssql_database" "sqlDb" {
  name           = "classifiedadsdevdb"
  server_id      = azurerm_mssql_server.sqlserver.id
  sku_name       = "Basic"

  tags = local.common_tags
}

data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "keyVault" {
  name                        = "classifiedadsdev"
  location                    = azurerm_resource_group.rg.location
  resource_group_name         = azurerm_resource_group.rg.name
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  sku_name = "standard"
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id
    key_permissions = ["Get", "List", "Create", "Delete", "Update", "Recover", "Purge", "GetRotationPolicy"]
  }

  tags = local.common_tags
}

resource "azurerm_service_plan" "appserviceplan" {
  name                = "ClassifiedAds-Hosts"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Windows"
  sku_name            = "B1"
  tags = local.common_tags
}

resource "azurerm_windows_web_app" "identityserver_classifiedads" {
  name                = "identityserver-classifiedads"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id = azurerm_service_plan.appserviceplan.id
  site_config {
    application_stack {
      current_stack   = "dotnet"
      dotnet_version  = "v8.0"
      }  
  }
  app_settings = {
    "ConnectionStrings__ClassifiedAds" = "Server=tcp:${var.sqlServerName}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.sqlDb.name};Persist Security Info=False;User ID=${var.sqlServerUserName};Password=${var.sqlServerPassword};MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;"
  }

  tags = local.common_tags
}

resource "azurerm_windows_web_app" "webapi_classifiedads" {
  name                = "webapi-classifiedads"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id = azurerm_service_plan.appserviceplan.id
  site_config {
    application_stack {
      current_stack   = "dotnet"
      dotnet_version  = "v8.0"
      }  
  }
  app_settings = {
    "ConnectionStrings__ClassifiedAds" = "Server=tcp:${var.sqlServerName}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.sqlDb.name};Persist Security Info=False;User ID=${var.sqlServerUserName};Password=${var.sqlServerPassword};MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;"
  }

  tags = local.common_tags
}

resource "azurerm_windows_web_app" "webmvc_classifiedads" {
  name                = "webmvc-classifiedads"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id = azurerm_service_plan.appserviceplan.id
  site_config {
    application_stack {
      current_stack   = "dotnet"
      dotnet_version  = "v8.0"
      }  
  }
  app_settings = {
    "ConnectionStrings__ClassifiedAds" = "Server=tcp:${var.sqlServerName}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.sqlDb.name};Persist Security Info=False;User ID=${var.sqlServerUserName};Password=${var.sqlServerPassword};MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;"
  }

  tags = local.common_tags
}