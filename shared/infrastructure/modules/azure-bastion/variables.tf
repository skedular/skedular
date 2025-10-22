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
