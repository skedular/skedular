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

variable "resource_group" {
  description = "Azure respurce group name."
  type        = string
  default     = null
}

variable "allowed_subnet_names" {
  type    = list(string)
  default = ["aks-public-subnet", "aks-private-subnet", "AzureBastionSubnet"]
}

variable "eventhubs" {
  description = "List of event hubs with config"
  type = list(object({
    name              = string
    partition_count   = number
    message_retention = number
  }))
  default = []
}