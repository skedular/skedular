variable "environment" {
  type        = string
  description = "environment name"
}

variable "google_analytics_measurement_id" {
  type        = string
  description = "Google Analytics measurement ID for the public website."
  default     = ""
}

variable "logrocket_app_id" {
  type        = string
  description = "LogRocket app ID for the public website."
  default     = ""
}
