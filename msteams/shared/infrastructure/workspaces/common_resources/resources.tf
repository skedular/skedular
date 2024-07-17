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

  web {
    redirect_uris = [
      "https://${module.shared_common.msteams_webapp_domain_name}/auth-end.html"
    ]
  }

  single_page_application {
    redirect_uris = [
      "https://${module.shared_common.msteams_webapp_domain_name}/api/auth/callback/msteams"
    ]
  }
}

resource "azuread_application_identifier_uri" "msteams_identifier_uris" {
  application_id = azuread_application.msteams.id
  identifier_uri = "api://${module.shared_common.msteams_webapp_domain_name}/${azuread_application.msteams.client_id}"

  depends_on = [azuread_application.msteams]
}

locals {
  access_as_user_id = uuid()
}

resource "azuread_application_permission_scope" "access_as_user" {
  application_id             = azuread_application.msteams.id
  scope_id                   = local.access_as_user_id
  admin_consent_display_name = "Teams can access app's web APIs"
  admin_consent_description  = "Allows Teams to call the app's web APIs as the current user."
  type                       = "User"
  user_consent_display_name  = "Teams can access app's web APIs and make requests on your behalf"
  user_consent_description   = "Enable Teams to call this app's web APIs with the same rights that you have"
  value                      = "access_as_user"

  depends_on = [azuread_application_identifier_uri.msteams_identifier_uris]
}


resource "azuread_application_pre_authorized" "team_desktop_mobile_client" {
  application_id       = azuread_application.msteams.id
  authorized_client_id = "1fec8e78-bce4-4aaf-ab1b-5451cc387264"

  permission_ids = [
    local.access_as_user_id,
  ]

  depends_on = [azuread_application_permission_scope.access_as_user]
}

resource "azuread_application_pre_authorized" "team_web_client" {
  application_id       = azuread_application.msteams.id
  authorized_client_id = "5e3ce6c0-2b1f-4285-8d4b-75ee78787346"

  permission_ids = [
    local.access_as_user_id,
  ]

  depends_on = [azuread_application_permission_scope.access_as_user]
}

resource "azuread_application_pre_authorized" "outlook_desktop_client" {
  application_id       = azuread_application.msteams.id
  authorized_client_id = "d3590ed6-52b3-4102-aeff-aad2292ab01c"

  permission_ids = [
    local.access_as_user_id,
  ]

  depends_on = [azuread_application_permission_scope.access_as_user]
}

resource "azuread_application_pre_authorized" "outlook_web_client_1" {
  application_id       = azuread_application.msteams.id
  authorized_client_id = "00000002-0000-0ff1-ce00-000000000000"

  permission_ids = [
    local.access_as_user_id,
  ]

  depends_on = [azuread_application_permission_scope.access_as_user]
}

resource "azuread_application_pre_authorized" "outlook_web_client_2" {
  application_id       = azuread_application.msteams.id
  authorized_client_id = "bc59ab01-8403-45c6-8796-ac3ef710b3e3"

  permission_ids = [
    local.access_as_user_id,
  ]

  depends_on = [azuread_application_permission_scope.access_as_user]
}

resource "azuread_application_pre_authorized" "ms365_app_desktop_client" {
  application_id       = azuread_application.msteams.id
  authorized_client_id = "0ec893e0-5785-4de6-99da-4ed124e5296c"

  permission_ids = [
    local.access_as_user_id,
  ]

  depends_on = [azuread_application_permission_scope.access_as_user]
}

resource "azuread_application_pre_authorized" "ms365_app_client_1" {
  application_id       = azuread_application.msteams.id
  authorized_client_id = "4345a7b9-9a63-4910-a426-35363201d503"

  permission_ids = [
    local.access_as_user_id,
  ]

  depends_on = [azuread_application_permission_scope.access_as_user]
}

resource "azuread_application_pre_authorized" "ms365_app_client_2" {
  application_id       = azuread_application.msteams.id
  authorized_client_id = "4765445b-32c6-49b0-83e6-1d93765276ca"

  permission_ids = [
    local.access_as_user_id,
  ]

  depends_on = [azuread_application_permission_scope.access_as_user]
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
