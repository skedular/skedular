variable "subscription_id" {
  description = "Azure subscription ID to deploy resources into."
  type        = string
  default     = null
}

variable "environment" {
  description = "Environment name (e.g. environment code name)."
  type        = string
  default     = null
}

variable "region" {
  description = "Azure region for resource deployment."
  type        = string
  default     = null
}

variable "sku_name" {
  description = "Specifies the SKU Name for this PostgreSQL Server."
  type        = string
  default     = null
}

variable "storage_mb" {
  description = "Specifies Max storage allowed for a server."
  type        = number
  default     = null
}

variable "administrator_login" {
  description = "The Administrator Login for the PostgreSQL Server."
  type        = string
  default     = null
}

variable "administrator_password" {
  description = "The Administrator Password for the PostgreSQL Server."
  type        = string
  default     = null
}
