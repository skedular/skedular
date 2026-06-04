output "project_name" {
  value = "${var.environment}-public-web"
}

output "domain_name" {
  value = var.environment == "production" ? "public.getskedular.com" : "stagingpublic.getskedular.com"
}

output "tags" {
  description = "Common tags"
  value = {
    domain = "public-web"
  }
}
