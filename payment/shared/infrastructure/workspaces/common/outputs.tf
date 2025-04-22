output "tags" {
  description = "Common tags"
  value = {
    domain = "payment"
  }
}

output "parameter_store_name_stripe_webhook_secret" {
  value = "stripe_webhook_secret"
}
