module "common" {
  source = "../common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

data "azuread_application_published_app_ids" "well_known" {}

data "azuread_service_principal" "msgraph" {
  client_id = data.azuread_application_published_app_ids.well_known.result["MicrosoftGraph"]
}

resource "azuread_application" "msteams" {
  display_name     = "msteams-${var.environment}"
  sign_in_audience = "AzureADMultipleOrgs"

  api {
    mapped_claims_enabled          = true
    requested_access_token_version = 2
  }

  optional_claims {
    access_token {
      name = "idtyp"
    }
  }

  feature_tags {
    enterprise = true
    gallery    = true
  }

  required_resource_access {
    resource_app_id = data.azuread_application_published_app_ids.well_known.result["MicrosoftGraph"]

    resource_access {
      id   = data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["User.Read"]
      type = "Scope"
    }

    resource_access {
      id   = data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["User.ReadBasic.All"]
      type = "Scope"
    }
  }

  single_page_application {
    redirect_uris = [
      "http://localhost:15002/api/auth/callback/msteams",
      "https://${module.shared_common.msteams_webapp_domain_name}/api/auth/callback/msteams",
      "https://mmsteams.unityhub.io/api/auth/callback/msteams"
    ]
  }
}

resource "azuread_application_identifier_uri" "msteams_identifier_uris" {
  application_id = azuread_application.msteams.id
  identifier_uri = "api://${azuread_application.msteams.client_id}"

  depends_on = [azuread_application.msteams]
}

resource "azuread_application_permission_scope" "access_as_user" {
  application_id             = azuread_application.msteams.id
  scope_id                   = uuid()
  admin_consent_display_name = "Teams can access app's web APIs"
  admin_consent_description  = "Allows Teams to call the app's web APIs as the current user."
  type                       = "User"
  user_consent_display_name  = "Teams can access app's web APIs and make requests on your behalf"
  user_consent_description   = "Enable Teams to call this app's web APIs with the same rights that you have"
  value                      = "access_as_user"

  depends_on = [azuread_application_identifier_uri.msteams_identifier_uris]
}

resource "aws_ssm_parameter" "msteams" {
  name  = module.common.parameter_store_name_azure_msteams_application_id
  type  = "String"
  value = azuread_application.msteams.client_id
  tags  = local.tags
}

resource "azuread_application_password" "msteams" {
  application_id = azuread_application.msteams.id
}

resource "aws_ssm_parameter" "msteams_secret_id" {
  name  = module.common.parameter_store_name_azure_msteams_secret_id
  type  = "String"
  value = azuread_application_password.msteams.key_id
  tags  = local.tags
}

resource "aws_ssm_parameter" "msteams_secret_value" {
  name  = module.common.parameter_store_name_azure_msteams_secret_value
  type  = "String"
  value = azuread_application_password.msteams.value
  tags  = local.tags
}
