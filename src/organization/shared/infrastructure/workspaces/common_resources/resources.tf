module "common" {
  source = "../common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

resource "stripe_webhook_endpoint" "webhook_platform_account" {
  url         = "https://${module.shared_common.api_domain_name}/v1/organization/stripe/platform/account/webhook"
  description = "Stripe Platform Account Webhook for Skedular - Organization Domain"
  connect     = false
  api_version = "2025-03-31.basil"
  enabled_events = [
    "account.updated"
  ]
}

resource "aws_ssm_parameter" "stripe_webhook_platform_account_secret" {
  name  = module.common.parameter_store_name_stripe_webhook_platform_account_secret
  type  = "String"
  value = stripe_webhook_endpoint.webhook_platform_account.secret
  tags  = local.tags
}

resource "stripe_webhook_endpoint" "webhook_connect_account" {
  url         = "https://${module.shared_common.api_domain_name}/v1/organization/stripe/connect/account/webhook"
  description = "Stripe Connect Account Webhook for Skedular - Organization Domain"
  connect     = true
  api_version = "2025-03-31.basil"
  enabled_events = [
    "account.application.authorized",
    "account.application.deauthorized",
    "account.external_account.created",
    "account.external_account.updated",
    "account.external_account.deleted",
    "account.updated",
    "checkout.session.completed",
    "checkout.session.expired",
    "checkout.session.async_payment_succeeded",
    "checkout.session.async_payment_failed",
    "customer.created",
    "customer.updated",
    "customer.deleted",
    "charge.pending",
    "charge.succeeded",
    "charge.updated",
    "charge.expired",
    "charge.failed",
  ]
}

resource "aws_ssm_parameter" "stripe_webhook_connect_account_secret" {
  name  = module.common.parameter_store_name_stripe_webhook_connect_account_secret
  type  = "String"
  value = stripe_webhook_endpoint.webhook_connect_account.secret
  tags  = local.tags
}
