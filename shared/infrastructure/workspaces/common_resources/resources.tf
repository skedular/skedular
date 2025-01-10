locals {
  is_staging = var.environment == "staging"
  dns_records_staging = [
    "apistaging",
    "billingapistaging",
    "bookingapistaging",
    "customerapistaging",
    "locationapistaging",
    "msteamsapistaging",
    "notificationapistaging",
    "organizationapistaging",
    "paymentapistaging",
    "slackapistaging",
    "teamapistaging"
  ]
  dns_records_production = [
    "api",
    "billingapi",
    "bookingapi",
    "customerapi",
    "locationapi",
    "msteamsapi",
    "notificationapi",
    "organizationapi",
    "paymentapi",
    "slackapi",
    "teamapi"
  ]
}

module "common" {
  source = "../common"

  environment = var.environment
}

module "simple_email_service" {
  source = "../../modules/aws_simple_email_service"
  providers = {
    aws        = aws
    cloudflare = cloudflare
  }

  tags              = local.tags
  domain            = module.common.simple_email_service_domain
  cloudflare_domain = module.common.cloudflare_webapp_domain_name
}

module "cognito_user_pool" {
  source = "../../modules/aws_cognito_user_pool"
  providers = {
    aws = aws
  }

  tags                              = local.tags
  name                              = module.common.cognito_user_pool_name
  domain                            = module.common.cognito_user_pool_domain
  simple_email_service_arn          = module.simple_email_service.arn
  from_email_address                = module.common.from_email_address
  reply_to_email_address            = module.common.reply_to_email_address
  gcp_web_credentials_client_id     = var.gcp_web_credentials_client_id
  gcp_web_credentials_client_secret = var.gcp_web_credentials_client_secret
  google_provider_name              = module.common.aws_cognito_identity_provider_google_provider_name
}

resource "stripe_product" "pay_as_you_go_v1" {
  name        = "Premium"
  unit_label  = "Active User"
  description = "Skedular Pay-as-you-go"
  url         = "https://${module.common.cloudflare_public_website_domain_name}/pricing"
  metadata = {
    offering_code = "PAY_AS_YOU_GO_V1"
  }
}

resource "stripe_price" "pay_as_you_go_v1_price_v1" {
  product     = stripe_product.pay_as_you_go_v1.id
  currency    = "usd"
  unit_amount = 300
  metadata = {
    offering_code = "PAY_AS_YOU_GO_V1"
  }
}

resource "aws_ssm_parameter" "stripe_pay_as_you_go_v1_product_id" {
  name  = module.common.parameter_store_name_stripe_pay_as_you_go_v1_product_id
  type  = "String"
  value = stripe_product.pay_as_you_go_v1.id
  tags  = local.tags
}

resource "aws_ssm_parameter" "stripe_pay_as_you_go_v1_product_unit_amount" {
  name  = module.common.parameter_store_name_stripe_pay_as_you_go_v1_product_unit_amount
  type  = "String"
  value = stripe_price.pay_as_you_go_v1_price_v1.unit_amount
  tags  = local.tags
}

data "cloudflare_zone" "public_website" {
  name = module.common.cloudflare_public_website_domain_name
}

resource "cloudflare_record" "cloudflare_dns_record_production" {
  count   = local.is_staging ? 0 : 1
  zone_id = data.cloudflare_zone.public_website.id
  name    = "@"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "cloudflare_dns_record_production_staging" {
  count   = local.is_staging ? 1 : 0
  zone_id = data.cloudflare_zone.public_website.id
  name    = "staging"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

data "cloudflare_zone" "webapp" {
  name = module.common.cloudflare_webapp_domain_name
}

resource "cloudflare_record" "cloudflare_dns_records_staging" {
  count   = local.is_staging ? length(local.dns_records_staging) : 0
  zone_id = data.cloudflare_zone.webapp.id
  name    = element(local.dns_records_staging, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "cloudflare_dns_records_production" {
  count   = local.is_staging ? 0 : length(local.dns_records_production)
  zone_id = data.cloudflare_zone.webapp.id
  name    = element(local.dns_records_production, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}
