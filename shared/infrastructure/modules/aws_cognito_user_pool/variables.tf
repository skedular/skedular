variable "tags" {
  type        = map(string)
  description = "tags"
}

variable "name" {
  type        = string
  description = "user pool name"
}

variable "domain" {
  type        = string
  description = "cognito domain"
}

variable "simple_email_service_arn" {
  type        = string
  description = "simple email service ARN"
}

variable "from_email_address" {
  type        = string
  description = "from email address"
}

variable "reply_to_email_address" {
  type        = string
  description = "reply to email address"
}

variable "google_provider_name" {
  type        = string
  description = "Google provider name"
}

variable "gcp_unityhub_web_credentials_client_id" {
  type        = string
  description = "GCP UnityHub web credentials client Id"
}

variable "gcp_unityhub_web_credentials_client_secret" {
  type        = string
  description = "GCP UnityHub web credentials client Secret"
}
