locals {
  azure_app_display_name_dev = "UnityHub-dev"
  azure_app_description_dev  = "UnityHub-dev"
}

resource "azuread_application" "azure_application_dev" {
  count                 = local.is_staging ? 1 : 0
  display_name          = local.azure_app_display_name_dev
  description           = local.azure_app_description_dev
  sign_in_audience      = "AzureADMultipleOrgs"
  privacy_statement_url = "https://getskedular.com/privacy-policy"
  terms_of_service_url  = "https://getskedular.com/terms-of-service"
  logo_image            = filebase64("../../../../assets/logos/logo.png")

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

resource "azuread_application_redirect_uris" "azure_application_web_redirect_uris_dev" {
  count          = local.is_staging ? 1 : 0
  application_id = azuread_application.azure_application_dev[count.index].id
  type           = "Web"

  redirect_uris = [
    "http://localhost:15000/api/auth/callback/azure-ad"
  ]
}

resource "azuread_application_redirect_uris" "azure_application_spa_redirect_uris_dev" {
  count          = local.is_staging ? 1 : 0
  application_id = azuread_application.azure_application_dev[count.index].id
  type           = "SPA"

  redirect_uris = [
    "https://localhost:15002/auth-end.html?clientId=${azuread_application.azure_application_dev[count.index].client_id}",
    "http://localhost:10200/organization/api/v1/onboard-azure-tenant"
  ]
}

resource "azuread_application_identifier_uri" "azure_application_identifier_uris_dev" {
  count          = local.is_staging ? 1 : 0
  application_id = azuread_application.azure_application_dev[count.index].id
  identifier_uri = "api://localhost:15002/${azuread_application.azure_application_dev[count.index].client_id}"
}

resource "random_uuid" "access_as_user_dev_id" {
  count = local.is_staging ? 1 : 0
}

resource "azuread_application_permission_scope" "access_as_user_dev" {
  count                      = local.is_staging ? 1 : 0
  application_id             = azuread_application.azure_application_dev[count.index].id
  scope_id                   = random_uuid.access_as_user_dev_id[count.index].result
  admin_consent_display_name = "UnityHub application can access app's web APIs"
  admin_consent_description  = "Allows UnityHub application to call the app's web APIs as the current user."
  type                       = "User"
  user_consent_display_name  = "UnityHub application can access app's web APIs and make requests on your behalf"
  user_consent_description   = "Enable UnityHub application to call this app's web APIs with the same rights that you have"
  value                      = "access_as_user"
}

resource "azuread_application_pre_authorized" "pre_authorized_clients_dev" {
  count                = local.is_staging ? length(local.pre_authorized_client_ids) : 0
  application_id       = azuread_application.azure_application_dev[0].id
  authorized_client_id = local.pre_authorized_client_ids[count.index]

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[0].scope_id
  ]
}

resource "aws_ssm_parameter" "azure_application_dev" {
  count = local.is_staging ? 1 : 0
  name  = module.shared_common.parameter_store_name_azure_application_id_dev
  type  = "String"
  value = azuread_application.azure_application_dev[count.index].client_id
  tags  = local.tags
}

resource "azuread_application_password" "azure_application_dev" {
  count          = local.is_staging ? 1 : 0
  application_id = azuread_application.azure_application_dev[count.index].id
}

resource "aws_ssm_parameter" "azure_application_dev_secret_id" {
  count = local.is_staging ? 1 : 0
  name  = module.shared_common.parameter_store_name_azure_application_secret_id_dev
  type  = "String"
  value = azuread_application_password.azure_application_dev[count.index].key_id
  tags  = local.tags
}

resource "aws_ssm_parameter" "azure_application_dev_secret_value" {
  count = local.is_staging ? 1 : 0
  name  = module.shared_common.parameter_store_name_azure_application_secret_value_dev
  type  = "String"
  value = azuread_application_password.azure_application_dev[count.index].value
  tags  = local.tags
}
