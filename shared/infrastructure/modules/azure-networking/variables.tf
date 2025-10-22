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

variable "tags" {
  description = "A map of tags to apply to resources."
  type        = map(string)
  default     = {}
}

variable "key_permissions" {
  type        = list(string)
  description = "List of key permissions."
  default     = ["List", "Create", "Delete", "Get", "List", "Purge", "Recover", "Update", "GetRotationPolicy", "SetRotationPolicy"]
}

variable "secret_permissions" {
  type        = list(string)
  description = "List of secret permissions."
  default     = ["Set", "Get", "List", "Delete", "Purge", "Recover"]
}
