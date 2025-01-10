variable "cloudflare_api_key" {
  type    = string
  default = ""
}

variable "random_seed" {
  type        = string
  description = "will be used to re-trigger random password generation"
  default     = ""
}

variable "vercel_api_token" {
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
