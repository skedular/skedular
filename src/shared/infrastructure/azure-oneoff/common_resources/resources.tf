module "common" {
  source = "../../workspaces/common"

  environment = var.environment
}

resource "azuread_application_registration" "github_oidc" {
  display_name = "github-workflow-oidc-${var.environment}"
}

resource "aws_ssm_parameter" "github_oidc" {
  name  = module.common.parameter_store_name_azure_github_actions_oidc_application_id
  type  = "String"
  value = azuread_application_registration.github_oidc.client_id
  tags  = local.tags
}

resource "azuread_service_principal" "github_oidc" {
  client_id                    = azuread_application_registration.github_oidc.client_id
  app_role_assignment_required = false
}

locals {
  default_audience_name = "api://AzureADTokenExchange"
  github_issuer_url     = "https://token.actions.githubusercontent.com"
}


resource "azuread_application_federated_identity_credential" "github_oidc_main" {
  application_id = azuread_application_registration.github_oidc.id
  display_name   = "${var.environment}-unityhubio-unityhubio-main"
  description    = "Deployments for ${module.common.github_repository_unityhubio} for environment ${var.environment} and main branch"
  audiences      = [local.default_audience_name]
  issuer         = local.github_issuer_url
  subject        = "repo:${module.common.github_repository_unityhubio}:ref:refs/heads/main"
}

resource "azuread_application_federated_identity_credential" "github_oidc_pullrequest" {
  application_id = azuread_application_registration.github_oidc.id
  display_name   = "${var.environment}-unityhubio-unityhubio-pullrequest"
  description    = "Deployments for ${module.common.github_repository_unityhubio} for environment ${var.environment} and pullrequest"
  audiences      = [local.default_audience_name]
  issuer         = local.github_issuer_url
  subject        = "repo:${module.common.github_repository_unityhubio}:pull_request"
}

resource "azuread_application_federated_identity_credential" "github_oidc_environment" {
  application_id = azuread_application_registration.github_oidc.id
  display_name   = "${var.environment}-unityhubio-unityhubio-environment"
  description    = "Deployments for ${module.common.github_repository_unityhubio} for environment ${var.environment} and environment"
  audiences      = [local.default_audience_name]
  issuer         = local.github_issuer_url
  subject        = "repo:${module.common.github_repository_unityhubio}:environment:${var.environment}"
}
