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
