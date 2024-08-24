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

resource "azuread_application" "azure_application_dev" {
  count            = var.environment == "staging" ? 1 : 0
  display_name     = "UnityHub-dev"
  sign_in_audience = "AzureADMultipleOrgs"

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
    #   id   = data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["User.Read"]
    #   type = "Role"
    # }

    # resource_access {
    #   id   = data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["User.ReadBasic.All"]
    #   type = "Scope"
    # }

    resource_access {
      # User.ReadBasic.All
      id   = "97235f07-e226-4f63-ace3-39588e11d3a1"
      type = "Role"
    }
  }

  single_page_application {
    redirect_uris = [
    ]
  }
}

resource "azuread_application_redirect_uris" "azure_application_web_redirect_uris_dev" {
  count          = var.environment == "staging" ? 1 : 0
  application_id = azuread_application.azure_application_dev[count.index].id
  type           = "Web"

  redirect_uris = [
    "http://localhost:15000/api/auth/callback/azure-ad"
  ]
}

resource "azuread_application_redirect_uris" "azure_application_spa_redirect_uris_dev" {
  count          = var.environment == "staging" ? 1 : 0
  application_id = azuread_application.azure_application_dev[count.index].id
  type           = "SPA"

  redirect_uris = [
    # "https://localhost:15002/auth-end.html?clientId=${azuread_application.azure_application_dev[count.index].client_id}",
    "http://localhost:10200/organization/api/v1/onboard-azure-tenant"
  ]
}

resource "azuread_application_identifier_uri" "azure_application_identifier_uris_dev" {
  count          = var.environment == "staging" ? 1 : 0
  application_id = azuread_application.azure_application_dev[count.index].id
  identifier_uri = "api://localhost:15002/${azuread_application.azure_application_dev[count.index].client_id}"
}

resource "random_uuid" "access_as_user_dev_id" {
  count = var.environment == "staging" ? 1 : 0
}

resource "azuread_application_permission_scope" "access_as_user_dev" {
  count                      = var.environment == "staging" ? 1 : 0
  application_id             = azuread_application.azure_application_dev[count.index].id
  scope_id                   = random_uuid.access_as_user_dev_id[count.index].result
  admin_consent_display_name = "Teams can access app's web APIs"
  admin_consent_description  = "Allows Teams to call the app's web APIs as the current user."
  type                       = "User"
  user_consent_display_name  = "Teams can access app's web APIs and make requests on your behalf"
  user_consent_description   = "Enable Teams to call this app's web APIs with the same rights that you have"
  value                      = "access_as_user"
}

resource "azuread_application_pre_authorized" "team_desktop_mobile_client_dev" {
  count                = var.environment == "staging" ? 1 : 0
  application_id       = azuread_application.azure_application_dev[count.index].id
  authorized_client_id = "1fec8e78-bce4-4aaf-ab1b-5451cc387264"

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[count.index].scope_id,
  ]
}

resource "azuread_application_pre_authorized" "team_web_client_dev" {
  count                = var.environment == "staging" ? 1 : 0
  application_id       = azuread_application.azure_application_dev[count.index].id
  authorized_client_id = "5e3ce6c0-2b1f-4285-8d4b-75ee78787346"

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[count.index].scope_id,
  ]
}

resource "azuread_application_pre_authorized" "outlook_desktop_client_dev" {
  count                = var.environment == "staging" ? 1 : 0
  application_id       = azuread_application.azure_application_dev[count.index].id
  authorized_client_id = "d3590ed6-52b3-4102-aeff-aad2292ab01c"

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[count.index].scope_id,
  ]
}

resource "azuread_application_pre_authorized" "outlook_web_client_1_dev" {
  count                = var.environment == "staging" ? 1 : 0
  application_id       = azuread_application.azure_application_dev[count.index].id
  authorized_client_id = "00000002-0000-0ff1-ce00-000000000000"

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[count.index].scope_id,
  ]
}

resource "azuread_application_pre_authorized" "outlook_web_client_2_dev" {
  count                = var.environment == "staging" ? 1 : 0
  application_id       = azuread_application.azure_application_dev[count.index].id
  authorized_client_id = "bc59ab01-8403-45c6-8796-ac3ef710b3e3"

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[count.index].scope_id,
  ]
}

resource "azuread_application_pre_authorized" "ms365_app_desktop_client_dev" {
  count                = var.environment == "staging" ? 1 : 0
  application_id       = azuread_application.azure_application_dev[count.index].id
  authorized_client_id = "0ec893e0-5785-4de6-99da-4ed124e5296c"

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[count.index].scope_id,
  ]
}

resource "azuread_application_pre_authorized" "ms365_app_client_1_dev" {
  count                = var.environment == "staging" ? 1 : 0
  application_id       = azuread_application.azure_application_dev[count.index].id
  authorized_client_id = "4345a7b9-9a63-4910-a426-35363201d503"

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[count.index].scope_id,
  ]
}

resource "azuread_application_pre_authorized" "ms365_app_client_2_dev" {
  count                = var.environment == "staging" ? 1 : 0
  application_id       = azuread_application.azure_application_dev[count.index].id
  authorized_client_id = "4765445b-32c6-49b0-83e6-1d93765276ca"

  permission_ids = [
    azuread_application_permission_scope.access_as_user_dev[count.index].scope_id,
  ]
}

resource "aws_ssm_parameter" "azure_application_dev" {
  count = var.environment == "staging" ? 1 : 0
  name  = module.common.parameter_store_name_azure_application_id_dev
  type  = "String"
  value = azuread_application.azure_application_dev[count.index].client_id
  tags  = local.tags
}

resource "azuread_application_password" "azure_application_dev" {
  count          = var.environment == "staging" ? 1 : 0
  application_id = azuread_application.azure_application_dev[count.index].id
}

resource "aws_ssm_parameter" "azure_application_dev_secret_id" {
  count = var.environment == "staging" ? 1 : 0
  name  = module.common.parameter_store_name_azure_application_secret_id_dev
  type  = "String"
  value = azuread_application_password.azure_application_dev[count.index].key_id
  tags  = local.tags
}

resource "aws_ssm_parameter" "azure_application_dev_secret_value" {
  count = var.environment == "staging" ? 1 : 0
  name  = module.common.parameter_store_name_azure_application_secret_value_dev
  type  = "String"
  value = azuread_application_password.azure_application_dev[count.index].value
  tags  = local.tags
}
