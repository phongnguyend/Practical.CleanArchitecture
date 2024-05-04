variable "storageAccountName" {
  type        = string
  description = "Storage Account Name"
  default     = "classifiedadsdev"
}

variable "sqlServerName" {
  type        = string
  description = "Sql Server Name"
  default     = "classifiedadsdev"
}

variable "sqlServerUserName" {
  type        = string
  description = "Sql Server UserName "
  default     = "classifiedads"
}

variable "sqlServerPassword" {
  type        = string
  description = "Sql Server Password"
  default     = "sqladmin123!@#"
}