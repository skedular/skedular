variable "cloudflare_api_key" {
  type      = string
  sensitive = true
  default   = ""
}

variable "random_seed" {
  type        = string
  description = "will be used to re-trigger random password generation"
  default     = ""
}
