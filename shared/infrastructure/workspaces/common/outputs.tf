locals {
  is_production = var.environment == "production"

  public_website_domain_root = "getskedular.com"
  webapp_domain_root         = "skedular.app"
  public_website_domain      = local.is_production ? local.public_website_domain_root : "${var.environment}.${local.public_website_domain_root}"
  webapp_domain              = local.is_production ? "${local.webapp_domain_root}" : "${var.environment}.${local.webapp_domain_root}"
  api_domain                 = local.is_production ? "api.${local.webapp_domain_root}" : "api${var.environment}.${local.webapp_domain_root}"
  eventcatalog_webapp_domain = local.is_production ? "eventcatalog.${local.webapp_domain_root}" : "eventcatalog.${var.environment}.${local.webapp_domain_root}"
  cloudflarecdn              = local.is_production ? "cloudflarecdn" : "cloudflarecdn${var.environment}"
  awscdn                     = local.is_production ? "awscdn" : "awscdn${var.environment}"
}

output "cloudflare_account_id" {
  value = "26b0f35cc7cf1dd7be5973e8905fbfe8"
}

output "cloudflare_public_website_zone_id" {
  value = "3940392e8a2fbdc76f317f350df1f146"
}

output "cloudflare_webapp_zone_id" {
  value = "46f499b86f30ee281e480d41a4ad8a57"
}

output "cloudflare_public_website_domain_name" {
  value = local.public_website_domain_root
}

output "cloudflare_webapp_domain_name" {
  value = local.webapp_domain_root
}

output "cloudflare_webapp_cloudflare_cdn_domain_name" {
  value = local.cloudflarecdn
}

output "cloudflare_webapp_cloudflare_cdn_full_domain_name" {
  value = "${local.cloudflarecdn}.${local.webapp_domain_root}"
}

output "cloudflare_webapp_aws_cdn_domain_name" {
  value = local.awscdn
}

output "cloudflare_webapp_aws_cdn_full_domain_name" {
  value = "${local.awscdn}.${local.webapp_domain_root}"
}

output "aws_region" {
  value = "us-east-1"
}

output "azure_region" {
  value = "eastus"
}

output "gcp_region" {
  value = "us-east1"
}

output "cognito_user_pool_name" {
  value = var.environment
}

output "cognito_user_pool_domain" {
  value = local.is_production ? "skedular" : "${var.environment}skedular"
}

output "simple_email_service_domain" {
  value = local.webapp_domain
}

output "from_email_address" {
  value = "no-reply@${local.webapp_domain}"
}

output "reply_to_email_address" {
  value = "no-reply@${local.webapp_domain}"
}

output "api_domain_name" {
  value = local.api_domain
}

output "webapp_domain_name" {
  value = local.webapp_domain
}

output "eventcatalog_webapp_domain_name" {
  value = local.eventcatalog_webapp_domain
}

output "gcp_project_id" {
  value = "unityhub-${var.environment}"
}

output "github_repository" {
  value = "unityhubio/unityhubio"
}

output "aws_cognito_identity_provider_cognito_provider_name" {
  value = "COGNITO"
}

output "aws_cognito_identity_provider_google_provider_name" {
  value = "Google"
}

output "parameter_store_name_gcp_github_actions_workload_identity_provider" {
  value = "gcp_github_actions_workload_identity_provider"
}

output "parameter_store_name_gcp_github_actions_service_account" {
  value = "gcp_github_actions_service_account"
}

output "parameter_store_name_aws_github_actions_assume_role_arn" {
  value = "github_actions_assume_role_arn"
}

output "parameter_store_name_aws_github_actions_unityhubio_unityhubioassume_role_arn" {
  value = "github_actions_unityhubio_unityhubio_assume_role_arn"
}

output "logrocket_webapp_app_id" {
  value = "skedular/skedular-web-app-${var.environment}"
}

output "microanalytics_webapp_app_id" {
  value = local.is_production ? "ZwSg9rf6GA" : "ZwSg9rf6GA"
}

output "parameter_store_name_stripe_pay_as_you_go_v1_product_id" {
  value = "stripe_pay_as_you_go_v1_product_id"
}

output "parameter_store_name_stripe_pay_as_you_go_v1_product_unit_amount" {
  value = "stripe_pay_as_you_go_v1_product_unit_amount"
}

output "slack_app_id" {
  value = local.is_production ? "A05H015F9QE" : "A05H0126U7Q"
}

output "slack_client_id" {
  value = local.is_production ? "118234978193.5578039519830" : "118234978193.5578036232262"
}

output "api_gateway_name" {
  value = var.environment
}

output "parameter_store_name_azure_github_actions_oidc_application_id" {
  value = "azure_github_actions_oidc_application_id"
}

output "parameter_store_name_azure_application_id" {
  value = "azure_application_id"
}

output "parameter_store_name_azure_application_secret_id" {
  value = "azure_application_secret_id"
}

output "parameter_store_name_azure_application_secret_value" {
  value = "azure_application_secret_value"
}

output "parameter_store_name_azure_application_id_dev" {
  value = "azure_application_id_dev"
}

output "parameter_store_name_azure_application_secret_id_dev" {
  value = "azure_application_secret_id_dev"
}

output "parameter_store_name_azure_application_secret_value_dev" {
  value = "azure_application_secret_value_dev"
}

output "workos_client_id" {
  value = local.is_production ? "client_01H0Q195RQETQ0NFNDPE3GWBQK" : "client_01H0Q195NGRARQSJT1TTDAKKR9"
}
