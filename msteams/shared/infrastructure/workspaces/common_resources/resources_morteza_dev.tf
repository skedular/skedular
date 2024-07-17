resource "azuread_application" "msteams_morteza" {
  count            = var.environment == "staging" ? 1 : 0
  display_name     = "msteams-dev-morteza"
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
      "http://localhost:15002/auth-end.html",
      "https://mmsteams.unityhub.io/auth-end.html"
    ]
  }

  single_page_application {
    redirect_uris = [
      "https://mmsteams.unityhub.io/api/auth/callback/msteams"
    ]
  }
}

resource "azuread_application_identifier_uri" "msteams_identifier_uris_morteza" {
  count          = var.environment == "staging" ? 1 : 0
  application_id = azuread_application.msteams_morteza[count.index].id
  identifier_uri = "api://mmsteams.unityhub.io/${azuread_application.msteams_morteza[count.index].client_id}"

  depends_on = [azuread_application.msteams_morteza]
}

resource "azuread_application_permission_scope" "access_as_user_morteza" {
  count                      = var.environment == "staging" ? 1 : 0
  application_id             = azuread_application.msteams_morteza[count.index].id
  scope_id                   = uuid()
  admin_consent_display_name = "Teams can access app's web APIs"
  admin_consent_description  = "Allows Teams to call the app's web APIs as the current user."
  type                       = "User"
  user_consent_display_name  = "Teams can access app's web APIs and make requests on your behalf"
  user_consent_description   = "Enable Teams to call this app's web APIs with the same rights that you have"
  value                      = "access_as_user"

  depends_on = [azuread_application_identifier_uri.msteams_identifier_uris_morteza]
}
