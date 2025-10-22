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