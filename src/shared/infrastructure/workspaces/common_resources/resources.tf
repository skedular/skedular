locals {
  is_staging             = var.environment == "staging"
  dns_records_staging    = ["apistaging", "slackapistaging"]
  dns_records_production = ["api", "slackapi"]
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

  tags               = local.tags
  domain             = module.common.simple_email_service_domain
  cloudflare_zone_id = module.common.cloudflare_webapp_zone_id
  cloudflare_domain  = module.common.cloudflare_webapp_domain_name
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

resource "cloudflare_dns_record" "cloudflare_dns_record_production_www_webapp" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_webapp_zone_id
  name    = "www"
  content = "31.220.100.177"
  type    = "A"
  proxied = true
  ttl     = 1
}

resource "cloudflare_dns_record" "cloudflare_dns_records_staging" {
  count   = local.is_staging ? length(local.dns_records_staging) : 0
  zone_id = module.common.cloudflare_webapp_zone_id
  name    = element(local.dns_records_staging, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_dns_record" "cloudflare_dns_records_production" {
  count   = local.is_staging ? 0 : length(local.dns_records_production)
  zone_id = module.common.cloudflare_webapp_zone_id
  name    = element(local.dns_records_production, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_dns_record" "public_website_azure_custom_domain" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "@"
  content = "\"MS=ms14170435\""
  type    = "TXT"
  proxied = false
  ttl     = 3600
}

resource "cloudflare_dns_record" "webapp_azure_custom_domain" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_webapp_zone_id
  name    = "@"
  content = "\"MS=ms29548806\""
  type    = "TXT"
  proxied = false
  ttl     = 3600
}

resource "cloudflare_dns_record" "spaceship_public_website_srv" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "_autodiscover._tcp.${module.common.cloudflare_public_website_domain_name}"
  type    = "SRV"

  data = {
    service  = "_autodiscover"
    proto    = "_tcp"
    name     = module.common.cloudflare_public_website_domain_name
    priority = 0
    weight   = 0
    port     = 443
    target   = "autoconfig.spacemail.com"
  }
  proxied = false
  ttl     = 1200
}

resource "cloudflare_dns_record" "spaceship_public_website_mx_1" {
  count    = local.is_staging ? 0 : 1
  zone_id  = module.common.cloudflare_public_website_zone_id
  name     = "@"
  content  = "mx1.spacemail.com"
  type     = "MX"
  proxied  = false
  ttl      = 1200
  priority = 10
}

resource "cloudflare_dns_record" "spaceship_public_website_mx_2" {
  count    = local.is_staging ? 0 : 1
  zone_id  = module.common.cloudflare_public_website_zone_id
  name     = "@"
  content  = "mx2.spacemail.com"
  type     = "MX"
  proxied  = false
  ttl      = 1200
  priority = 10
}

resource "cloudflare_dns_record" "spaceship_public_website_spf" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "@"
  content = "\"v=spf1 include:spf.spacemail.com ~all\""
  type    = "TXT"
  proxied = false
  ttl     = 1200
}

resource "cloudflare_dns_record" "spaceship_public_website_domain_key" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "spacemail._domainkey"
  content = "\"v=DKIM1;k=rsa;p=MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxZjRvEBVPMWyyB8rSk4U3fszE48BpiUY/7byuVqSXG+ZmMrpU249AfQ70+NSAQxpxcU/5nKsx/BNSaYjFMS51IjCWeJg0I3EaTxeRsGirPO0GhYHpexV33KXOaJju8iDA2kLgr9BT0OYRD6m24uN00y+VA52JBexGBvlgmLh6KoIiKR+6pqURyhi/qw7aLqjX7ZKzEAtZHHvCCJCOyzurxpxTBVEco5zreGerrKkHr5LP+z59DY6xXtt4F2MMolS85sVCtYtJ+JPtHE8d5jJgKFvKPv7vNcgD3q2KJdECGkNskFYGyr0Hzf/NR2N7gZFYFmyRBv30VbEWJb4lzpn2QIDAQAB\""
  type    = "TXT"
  proxied = false
  ttl     = 1200
}

resource "cloudflare_dns_record" "spaceship_public_website_domain_verification" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "@"
  content = "\"297dd8c8-b379-40ec-b71a-d9175d8e2c13\""
  type    = "TXT"
  proxied = false
  ttl     = 1800
}

resource "cloudflare_dns_record" "public_website_dmarc" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "_dmarc"
  content = "\"v=DMARC1; p=reject; rua=mailto:dmarc-reports@${module.common.cloudflare_public_website_domain_name}; ruf=mailto:dmarc-failures@${module.common.cloudflare_public_website_domain_name}; aspf=r; sp=reject;\""
  type    = "TXT"
  proxied = false
  ttl     = 3600
}

resource "cloudflare_dns_record" "webapp_gmail_aws_ses_spf" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_webapp_zone_id
  name    = "@"
  content = "\"v=spf1 include:amazonses.com ~all\""
  type    = "TXT"
  proxied = false
  ttl     = 3600
}

resource "cloudflare_dns_record" "bing_verification" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "d4b213988b9e9e47f7d9f17ea01d5b38"
  content = "verify.bing.com"
  type    = "CNAME"
  proxied = false
  ttl     = 600
}

resource "cloudflare_dns_record" "yandex_verification" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "@"
  content = "\"yandex-verification: 47a94d9de0bdc184\""
  type    = "TXT"
  proxied = false
  ttl     = 3600
}

resource "cloudflare_dns_record" "saas_browser_verification" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "@"
  content = "\"saas-browser-verification=eyJhbGciOiJIUzI1NiJ9.eyJzZXJpYWxfaWQiOjE0NTQ3MzUsInVzZXJfaWQiOiJlMmZhM2U3NS0zMDMzLTRjNjMtODVmNC02YTZmNTAzNjZjOTMiLCJleHAiOjE3ODk3MTg5MDl9.cCYyUx7pyp5W5Ry3gQ88EK74gQako7YA5ms24w2_BVM\""
  type    = "TXT"
  proxied = false
  ttl     = 3600
}

resource "cloudflare_dns_record" "github_public_website" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_public_website_zone_id
  name    = "_gh-skedular-e"
  content = "9ea66c5313"
  type    = "TXT"
  proxied = false
  ttl     = 1800
}

resource "cloudflare_dns_record" "github_webapp" {
  count   = local.is_staging ? 0 : 1
  zone_id = module.common.cloudflare_webapp_zone_id
  name    = "_gh-skedular-e"
  content = "ae181b5ec4"
  type    = "TXT"
  proxied = false
  ttl     = 1800
}
