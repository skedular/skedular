variable "cloudflare_api_key" {
  type    = string
  default = ""
}

variable "stripe_api_key" {
  type    = string
  default = ""
}

variable "gcp_skedular_web_credentials_client_id" {
  type        = string
  description = "GCP Skedular web credentials client Id"
  default     = ""
}

variable "gcp_skedular_web_credentials_client_secret" {
  type        = string
  description = "GCP Skedular web credentials client Secret"
  default     = ""
}
