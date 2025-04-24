output "tags" {
  description = "Common tags"
  value = {
    domain = "payment"
  }
}

output "parameter_store_name_stripe_webhook_platform_account_secret" {
  value = "stripe_webhook_platform_account_secret"
}

output "parameter_store_name_stripe_webhook_connect_account_secret" {
  value = "stripe_webhook_connect_account_secret"
}
