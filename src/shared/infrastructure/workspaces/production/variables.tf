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
  default     = "edce45e7-5697-4935-bd63-648e9e609083"
}

variable "azure_region" {
  description = "Azure region for resource deployment."
  type        = string
  default     = "Australia East"
}
