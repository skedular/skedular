output "project_name" {
  value = "${var.environment}-public-web"
}

output "domain_name" {
  value = var.environment == "production" ? "getskedular.com" : "staging.getskedular.com"
}

output "domain_names" {
  value = var.environment == "production" ? ["getskedular.com", "www.getskedular.com"] : ["staging.getskedular.com"]
}

output "tags" {
  description = "Common tags"
  value = {
    domain = "public-web"
  }
}
