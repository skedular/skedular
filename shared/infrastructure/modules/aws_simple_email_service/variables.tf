variable "tags" {
  type        = map(string)
  description = "tags"
}

variable "domain" {
  type        = string
  description = "domain"
}

variable "cloudflare_zone_id" {
  type        = string
  description = "cloudflare zone id"
}

variable "cloudflare_domain" {
  type        = string
  description = "cloudflare domain"
}
