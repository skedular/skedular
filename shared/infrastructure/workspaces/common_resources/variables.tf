variable "environment" {
  type        = string
  description = "environment name"
}

variable "gcp_unityhub_web_credentials_client_id" {
  type        = string
  description = "GCP UnityHub web credentials client Id"
}

variable "gcp_unityhub_web_credentials_client_secret" {
  type        = string
  description = "GCP UnityHub web credentials client Secret"
}

variable "log_retention" {
  description = "retention in days"
  type        = number
  default     = 7
}