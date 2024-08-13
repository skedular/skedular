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
  cloudflare_domain = module.common.cloudflare_domain_name
}

module "cognito_user_pool" {
  source = "../../modules/aws_cognito_user_pool"
  providers = {
    aws = aws
  }

  tags                                       = local.tags
  name                                       = module.common.cognito_user_pool_name
  domain                                     = module.common.cognito_user_pool_domain
  simple_email_service_arn                   = module.simple_email_service.arn
  from_email_address                         = module.common.from_email_address
  reply_to_email_address                     = module.common.reply_to_email_address
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

data "cloudflare_zone" "default" {
  name = module.common.cloudflare_domain_name
}

resource "cloudflare_record" "wordpress_publicwebsite" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "@" : "staging"
  value   = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "wordpress_test" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "public" : "stagingpublic"
  value   = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "api" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "api" : "apistaging"
  value   = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "slack_api" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "slackapi" : "slackapistaging"
  value   = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "contabo" {
  count   = var.environment == "staging" ? 1 : 0
  zone_id = data.cloudflare_zone.default.id
  name    = "contabo"
  value   = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "mweb" {
  count   = var.environment == "staging" ? 1 : 0
  zone_id = data.cloudflare_zone.default.id
  name    = "mweb"
  value   = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "mapp" {
  count   = var.environment == "staging" ? 1 : 0
  zone_id = data.cloudflare_zone.default.id
  name    = "mapp"
  value   = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}
