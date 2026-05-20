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

variable "google_map_api_key" {
  type        = string
  description = "Google Map API Key"
}

variable "workos_api_key" {
  type        = string
  description = "WorkOS API Key"
  sensitive   = true
}

variable "workos_client_id" {
  type        = string
  description = "WorkOS Client ID"
  default     = "client_01KS2BQKSYN7FD2W2RR9JB9E55"
}
