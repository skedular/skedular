locals {
  is_staging             = var.environment == "staging"
  dns_records_staging    = ["staging", "apistaging", "slackapistaging"]
  dns_records_production = ["api", "slackapi"]

  pre_authorized_client_ids = [
    "1fec8e78-bce4-4aaf-ab1b-5451cc387264", # team_desktop_mobile_client
    "5e3ce6c0-2b1f-4285-8d4b-75ee78787346", # team_web_client
    "d3590ed6-52b3-4102-aeff-aad2292ab01c", # outlook_desktop_client
    "00000002-0000-0ff1-ce00-000000000000", # outlook_web_client_1
    "bc59ab01-8403-45c6-8796-ac3ef710b3e3", # outlook_web_client_2
    "0ec893e0-5785-4de6-99da-4ed124e5296c", # ms365_app_desktop_client
    "4345a7b9-9a63-4910-a426-35363201d503", # ms365_app_client_1
    "4765445b-32c6-49b0-83e6-1d93765276ca", # ms365_app_client_2
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
  cloudflare_domain = module.common.cloudflare_webapp_domain_name_1
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
