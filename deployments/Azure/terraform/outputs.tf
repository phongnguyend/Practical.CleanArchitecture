output "primary_access_key" {
  value       = azurerm_storage_account.sa.primary_access_key
  sensitive   = true
  description = "The primary access key for the storage account."
}

output "sql_database_connection_string" {
  value       = "Server=tcp:${var.sqlServerName}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.sqlDb.name};Persist Security Info=False;User ID=${var.sqlServerUserName};Password=${var.sqlServerPassword};MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;"
}