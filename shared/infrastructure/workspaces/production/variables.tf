variable "cloudflare_api_key" {
  type    = string
  default = ""
}

variable "stripe_api_key" {
  type    = string
  default = ""
}

variable "gcp_web_credentials_client_id" {
  type        = string
  description = "GCP web credentials client Id"
  default     = ""
}

variable "gcp_web_credentials_client_secret" {
  type        = string
  description = "GCP web credentials client Secret"
  default     = ""
}

variable "azure_subscription_id" {
  description = "Azure subscription ID to deploy resources into."
  type        = string
  default     = "763dfea3-3b46-43a7-9e56-bacef018b4ba"
}

variable "azure_region" {
  description = "Azure region for resource deployment."
  type        = string
  default     = "Australia East"
}
