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

variable "aks_subnet_id" {
  description = "The ID of the subnet to deploy the AKS cluster into."
  type        = string
  default     = null
}

variable "kubernetes_version" {
  type    = string
  default = "1.33"
}

variable "orchestrator_version" {
  type    = string
  default = null
}