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

module "simple_email_service_1" {
  source = "../../modules/aws_simple_email_service"
  providers = {
    aws        = aws
    cloudflare = cloudflare
  }

  tags              = local.tags
  domain            = module.common.simple_email_service_domain_1
  cloudflare_domain = module.common.cloudflare_public_website_domain_name_1
}

module "simple_email_service_2" {
  source = "../../modules/aws_simple_email_service"
  providers = {
    aws        = aws
    cloudflare = cloudflare
  }

  tags              = local.tags
  domain            = module.common.simple_email_service_domain_2
  cloudflare_domain = module.common.cloudflare_public_website_domain_name_2
}

module "cognito_user_pool" {
  source = "../../modules/aws_cognito_user_pool"
  providers = {
    aws = aws
  }

  tags                                       = local.tags
  name                                       = module.common.cognito_user_pool_name
  domain                                     = module.common.cognito_user_pool_domain
  simple_email_service_arn                   = module.simple_email_service_1.arn
  from_email_address                         = module.common.from_email_address_1
  reply_to_email_address                     = module.common.reply_to_email_address_1
  gcp_unityhub_web_credentials_client_id     = var.gcp_unityhub_web_credentials_client_id
  gcp_unityhub_web_credentials_client_secret = var.gcp_unityhub_web_credentials_client_secret
  google_provider_name                       = module.common.aws_cognito_identity_provider_google_provider_name
}

resource "stripe_product" "pay_as_you_go_v1" {
  name        = "Premium"
  unit_label  = "Active User"
  description = "UnityHub Pay-as-you-go"
  url         = "https://unityhub.io/pricing"
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

data "cloudflare_zone" "public_website_1" {
  name = module.common.cloudflare_public_website_domain_name_1
}

resource "cloudflare_record" "cloudflare_dns_record_production_1" {
  count   = local.is_staging ? 0 : 1
  zone_id = data.cloudflare_zone.public_website_1.id
  name    = "@"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "cloudflare_dns_record_production_1_staging" {
  count   = local.is_staging ? 1 : 0
  zone_id = data.cloudflare_zone.public_website_1.id
  name    = "staging"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

data "cloudflare_zone" "webapp_1" {
  name = module.common.cloudflare_webapp_domain_name_1
}

resource "cloudflare_record" "cloudflare_dns_records_staging_1" {
  count   = local.is_staging ? length(local.dns_records_staging) : 0
  zone_id = data.cloudflare_zone.webapp_1.id
  name    = element(local.dns_records_staging, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "cloudflare_dns_records_production_1" {
  count   = local.is_staging ? 0 : length(local.dns_records_production)
  zone_id = data.cloudflare_zone.webapp_1.id
  name    = element(local.dns_records_production, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

data "cloudflare_zone" "public_website_2" {
  name = module.common.cloudflare_public_website_domain_name_2
}

resource "cloudflare_record" "cloudflare_dns_record_production_2" {
  count   = local.is_staging ? 0 : 1
  zone_id = data.cloudflare_zone.public_website_2.id
  name    = "@"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "cloudflare_dns_record_production_2_staging" {
  count   = local.is_staging ? 1 : 0
  zone_id = data.cloudflare_zone.public_website_2.id
  name    = "staging"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

data "cloudflare_zone" "webapp_2" {
  name = module.common.cloudflare_webapp_domain_name_2
}

resource "cloudflare_record" "cloudflare_dns_records_staging_2" {
  count   = local.is_staging ? length(local.dns_records_staging) : 0
  zone_id = data.cloudflare_zone.webapp_2.id
  name    = element(local.dns_records_staging, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "cloudflare_dns_records_production_2" {
  count   = local.is_staging ? 0 : length(local.dns_records_production)
  zone_id = data.cloudflare_zone.webapp_2.id
  name    = element(local.dns_records_production, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}
