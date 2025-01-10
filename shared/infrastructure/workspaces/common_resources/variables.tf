variable "environment" {
  type        = string
  description = "environment name"
}

variable "gcp_web_credentials_client_id" {
  type        = string
  description = "GCP web credentials client Id"
}

variable "gcp_web_credentials_client_secret" {
  type        = string
  description = "GCP web credentials client Secret"
}

variable "log_retention" {
  description = "retention in days"
  type        = number
  default     = 7
}
