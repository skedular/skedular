module "common" {
  source = "../common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

resource "stripe_webhook_endpoint" "webhook" {
  url         = "https://${module.shared_common.api_domain_name}/payment/api/v1/stripe/webhook"
  description = "Stripe Webhook for Skedular"
  enabled_events = [
    "account.updated"
  ]
}

resource "aws_ssm_parameter" "stripe_webhook_secret" {
  name  = module.common.parameter_store_name_stripe_webhook_secret
  type  = "String"
  value = stripe_webhook_endpoint.webhook.secret
  tags  = local.tags
}
