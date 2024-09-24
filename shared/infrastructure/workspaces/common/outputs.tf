output "cloudflare_domain_name" {
  value = local.domain
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
  value = var.environment == "production" ? "unityhub" : "${var.environment}unityhub"
}

output "simple_email_service_domain" {
  value = var.environment == "production" ? local.domain : "${var.environment}.${local.domain}"
}

output "from_email_address" {
  value = var.environment == "production" ? "no-reply@${local.domain}" : "no-reply@${var.environment}.${local.domain}"
}

output "reply_to_email_address" {
  value = var.environment == "production" ? "no-reply@${local.domain}" : "no-reply@${var.environment}.${local.domain}"
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

output "domain_name" {
  value = var.environment == "production" ? local.domain : "${var.environment}.${local.domain}"
}

output "api_domain_name" {
  value = var.environment == "production" ? "api.${local.domain}" : "api${var.environment}.${local.domain}"
}

output "webapp_domain_name" {
  value = var.environment == "production" ? "app.${local.domain}" : "app.${var.environment}.${local.domain}"
}

output "msteams_webapp_domain_name" {
  value = var.environment == "production" ? "msteams.${local.domain}" : "msteams.${var.environment}.${local.domain}"
}

output "eventcatalog_webapp_domain_name" {
  value = var.environment == "production" ? "eventcatalog.${local.domain}" : "eventcatalog.${var.environment}.${local.domain}"
}

output "logrocket_publicwebsite_app_id" {
  value = "unity-hub/unityhub-public-website-${var.environment}"
}

output "logrocket_webapp_app_id" {
  value = "unity-hub/unityhub-web-app-${var.environment}"
}

output "logrocket_msteams_webapp_app_id" {
  value = "unity-hub/unityhub-msteams-web-app-${var.environment}"
}

output "microanalytics_publicwebsite_app_id" {
  value = var.environment == "production" ? "ZwSg9rf6GA" : "ZwSg9rf6GA"
}

output "microanalytics_webapp_app_id" {
  value = var.environment == "production" ? "ZwSg9rf6GA" : "ZwSg9rf6GA"
}

output "parameter_store_name_stripe_pay_as_you_go_v1_product_id" {
  value = "stripe_pay_as_you_go_v1_product_id"
}

output "parameter_store_name_stripe_pay_as_you_go_v1_product_unit_amount" {
  value = "stripe_pay_as_you_go_v1_product_unit_amount"
}

output "slack_app_id" {
  value = var.environment == "production" ? "A05H015F9QE" : "A05H0126U7Q"
}

output "slack_client_id" {
  value = var.environment == "production" ? "118234978193.5578039519830" : "118234978193.5578036232262"
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
