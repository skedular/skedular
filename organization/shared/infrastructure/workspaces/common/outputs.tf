output "tags" {
  description = "Common tags"
  value = {
    domain = "organization"
  }
}

output "parameter_store_name_stripe_webhook_platform_account_secret" {
  value = "stripe_organization_webhook_platform_account_secret"
}

output "parameter_store_name_stripe_webhook_connect_account_secret" {
  value = "stripe_organization_webhook_connect_account_secret"
}
