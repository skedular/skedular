locals {
  is_staging = var.environment == "staging"

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

module "shared_common" {
  source = "../../../infrastructure/workspaces/common"

  environment = var.environment
}

data "azuread_application_published_app_ids" "well_known" {}

data "azuread_service_principal" "msgraph" {
  client_id = data.azuread_application_published_app_ids.well_known.result["MicrosoftGraph"]
}

resource "azuread_application" "azure_application" {
  display_name          = var.environment == "production" ? "Skedular" : "Skedular-${var.environment}"
  description           = var.environment == "production" ? "Skedular" : "Skedular-${var.environment}"
  sign_in_audience      = "AzureADMultipleOrgs"
  privacy_statement_url = "https://getskedular.com/privacy-policy"
  terms_of_service_url  = "https://getskedular.com/terms-of-service"
  logo_image            = filebase64("../../../../assets/logos/skedular-icon-primary.png")

  web {
    homepage_url = "https://getskedular.com"
  }

  api {
    mapped_claims_enabled          = true
    requested_access_token_version = 2
  }

  feature_tags {
    enterprise = true
    gallery    = true
  }

  required_resource_access {
    resource_app_id = data.azuread_application_published_app_ids.well_known.result["MicrosoftGraph"]

    # resource_access {
    #   id   = data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["User.ReadBasic.All"]
    #   type = "Scope"
    # }

    # resource_access {
    #   id   = data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["ProfilePhoto.Read.All"]
    #   type = "Scope"
    # }

    resource_access {
      id   = "97235f07-e226-4f63-ace3-39588e11d3a1" # User.ReadBasic.All
      type = "Role"
    }
    resource_access {
      id   = "e24d31aa-e1ab-4c80-85fe-23018690335d" # ProfilePhoto.Read.All
      type = "Role"
    }
    resource_access {
      id   = "243cded2-bd16-4fd6-a953-ff8177894c3d" # ChannelSettings.ReadWrite.All
      type = "Role"
    }
    resource_access {
      id   = "62a82d76-70ea-41e2-9197-370581804d09" # Group.ReadWrite.All
      type = "Role"
    }
    resource_access {
      id   = "2280dda6-0bfd-44ee-a2f4-cb867cfc4c1e" # Team.ReadBasic.All
      type = "Role"
    }
    resource_access {
      id   = "dfb0dd15-61de-45b2-be36-d6a69fba3c79" # Teamwork.Migrate.All
      type = "Role"
    }
  }
}

resource "azuread_application_redirect_uris" "azure_application_web_redirect_uris" {
  application_id = azuread_application.azure_application.id
  type           = "Web"

  redirect_uris = [
    "https://${module.shared_common.webapp_domain_name}/api/auth/callback/azure-ad"
  ]
}

resource "azuread_application_redirect_uris" "azure_application_spa_redirect_uris" {
  application_id = azuread_application.azure_application.id
  type           = "SPA"

  redirect_uris = [
    "https://${module.shared_common.webapp_domain_name}/auth-end.html?clientId=${azuread_application.azure_application.client_id}",
    "https://${module.shared_common.api_domain_name}/organization/api/v1/onboard-azure-tenant"
  ]
}

resource "azuread_application_identifier_uri" "azure_application_identifier_uris" {
  application_id = azuread_application.azure_application.id
  identifier_uri = "api://${module.shared_common.webapp_domain_name}/${azuread_application.azure_application.client_id}"
}

resource "random_uuid" "access_as_user_id" {}

resource "azuread_application_permission_scope" "access_as_user" {
  application_id             = azuread_application.azure_application.id
  scope_id                   = random_uuid.access_as_user_id.result
  admin_consent_display_name = "Skedular application can access app's web APIs"
  admin_consent_description  = "Allows Skedular application to call the app's web APIs as the current user."
  type                       = "User"
  user_consent_display_name  = "Skedular application can access app's web APIs and make requests on your behalf"
  user_consent_description   = "Enable Skedular application to call this app's web APIs with the same rights that you have"
  value                      = "access_as_user"
}

resource "azuread_application_pre_authorized" "pre_authorized_clients" {
  count                = length(local.pre_authorized_client_ids)
  application_id       = azuread_application.azure_application.id
  authorized_client_id = local.pre_authorized_client_ids[count.index]

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "aws_ssm_parameter" "azure_application" {
  name  = module.shared_common.parameter_store_name_azure_application_id
  type  = "String"
  value = azuread_application.azure_application.client_id
  tags  = local.tags
}

resource "azuread_application_password" "azure_application" {
  application_id = azuread_application.azure_application.id
}

resource "aws_ssm_parameter" "azure_application_secret_id" {
  name  = module.shared_common.parameter_store_name_azure_application_secret_id
  type  = "String"
  value = azuread_application_password.azure_application.key_id
  tags  = local.tags
}

resource "aws_ssm_parameter" "azure_application_secret_value" {
  name  = module.shared_common.parameter_store_name_azure_application_secret_value
  type  = "String"
  value = azuread_application_password.azure_application.value
  tags  = local.tags
}
