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
