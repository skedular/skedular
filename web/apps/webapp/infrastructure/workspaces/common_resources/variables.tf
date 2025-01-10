variable "environment" {
  type        = string
  description = "environment name"
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

variable "google_analytics_measurement_id" {
  type        = string
  description = "Google Analytics measurement id"
}

variable "google_tag_manager_container_id" {
  type        = string
  description = "Google Tag Manager container id"
}
