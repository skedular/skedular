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

  tags              = local.tags
  domain            = module.common.simple_email_service_domain
  cloudflare_domain = module.common.cloudflare_domain_name
}

module "cognito_user_pool" {
  source = "../../modules/aws_cognito_user_pool"
  providers = {
    aws = aws
  }

  tags                                       = local.tags
  name                                       = module.common.cognito_user_pool_name
  domain                                     = module.common.cognito_user_pool_domain
  simple_email_service_arn                   = module.simple_email_service.arn
  from_email_address                         = module.common.from_email_address
  reply_to_email_address                     = module.common.reply_to_email_address
  gcp_unityhub_web_credentials_client_id     = var.gcp_unityhub_web_credentials_client_id
  gcp_unityhub_web_credentials_client_secret = var.gcp_unityhub_web_credentials_client_secret
  google_provider_name                       = module.common.aws_cognito_identity_provider_google_provider_name
}

resource "stripe_product" "pay_as_you_go_v1" {
  name        = "Premium"
  unit_label  = "Active User"
  description = "UnityHub Pay-as-you-go"
  url         = "https://unityhub.io/pricing"
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

data "cloudflare_zone" "default" {
  name = module.common.cloudflare_domain_name
}

resource "cloudflare_record" "wordpress_publicwebsite" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "@" : "staging"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "wordpress_test" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "public" : "stagingpublic"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "api" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "api" : "apistaging"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "slack_api" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "slackapi" : "slackapistaging"
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

data "azuread_application_published_app_ids" "well_known" {}

data "azuread_service_principal" "msgraph" {
  client_id = data.azuread_application_published_app_ids.well_known.result["MicrosoftGraph"]
}

resource "azuread_application" "azure_application" {
  display_name          = var.environment == "production" ? "UnityHub" : "UnityHub-${var.environment}"
  description           = var.environment == "production" ? "UnityHub" : "UnityHub-${var.environment}"
  sign_in_audience      = "AzureADMultipleOrgs"
  privacy_statement_url = "https://unityhub.io/privacy-policy"
  terms_of_service_url  = "https://unityhub.io/terms-of-service"
  logo_image            = filebase64("../../../../assets/logos/logo.png")

  web {
    homepage_url = "https://unityhub.io"
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
      # User.ReadBasic.All
      id   = "97235f07-e226-4f63-ace3-39588e11d3a1"
      type = "Role"
    }

    resource_access {
      # ProfilePhoto.Read.All
      id   = "e24d31aa-e1ab-4c80-85fe-23018690335d"
      type = "Role"
    }

    resource_access {
      # ChannelSettings.ReadWrite.All
      id   = "243cded2-bd16-4fd6-a953-ff8177894c3d"
      type = "Role"
    }

    resource_access {
      # Group.ReadWrite.All
      id   = "62a82d76-70ea-41e2-9197-370581804d09"
      type = "Role"
    }

    resource_access {
      # Team.ReadBasic.All
      id   = "2280dda6-0bfd-44ee-a2f4-cb867cfc4c1e"
      type = "Role"
    }

    resource_access {
      # Teamwork.Migrate.All
      id   = "dfb0dd15-61de-45b2-be36-d6a69fba3c79"
      type = "Role"
    }
  }
}

resource "azuread_application_redirect_uris" "azure_application_web_redirect_uris" {
  application_id = azuread_application.azure_application.id
  type           = "Web"

  redirect_uris = [
    "https://${module.common.webapp_domain_name}/api/auth/callback/azure-ad"
  ]
}

resource "azuread_application_redirect_uris" "azure_application_spa_redirect_uris" {
  application_id = azuread_application.azure_application.id
  type           = "SPA"

  redirect_uris = [
    "https://${module.common.msteams_webapp_domain_name}/auth-end.html?clientId=${azuread_application.azure_application.client_id}",
    "https://${module.common.api_domain_name}/organization/api/v1/onboard-azure-tenant"
  ]
}

resource "azuread_application_identifier_uri" "azure_application_identifier_uris" {
  application_id = azuread_application.azure_application.id
  identifier_uri = "api://${module.common.msteams_webapp_domain_name}/${azuread_application.azure_application.client_id}"
}

resource "random_uuid" "access_as_user_id" {}

resource "azuread_application_permission_scope" "access_as_user" {
  application_id             = azuread_application.azure_application.id
  scope_id                   = random_uuid.access_as_user_id.result
  admin_consent_display_name = "UnityHub application can access app's web APIs"
  admin_consent_description  = "Allows UnityHub application to call the app's web APIs as the current user."
  type                       = "User"
  user_consent_display_name  = "UnityHub application can access app's web APIs and make requests on your behalf"
  user_consent_description   = "Enable UnityHub application to call this app's web APIs with the same rights that you have"
  value                      = "access_as_user"
}

resource "azuread_application_pre_authorized" "team_desktop_mobile_client" {
  application_id       = azuread_application.azure_application.id
  authorized_client_id = "1fec8e78-bce4-4aaf-ab1b-5451cc387264"

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "azuread_application_pre_authorized" "team_web_client" {
  application_id       = azuread_application.azure_application.id
  authorized_client_id = "5e3ce6c0-2b1f-4285-8d4b-75ee78787346"

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "azuread_application_pre_authorized" "outlook_desktop_client" {
  application_id       = azuread_application.azure_application.id
  authorized_client_id = "d3590ed6-52b3-4102-aeff-aad2292ab01c"

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "azuread_application_pre_authorized" "outlook_web_client_1" {
  application_id       = azuread_application.azure_application.id
  authorized_client_id = "00000002-0000-0ff1-ce00-000000000000"

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "azuread_application_pre_authorized" "outlook_web_client_2" {
  application_id       = azuread_application.azure_application.id
  authorized_client_id = "bc59ab01-8403-45c6-8796-ac3ef710b3e3"

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "azuread_application_pre_authorized" "ms365_app_desktop_client" {
  application_id       = azuread_application.azure_application.id
  authorized_client_id = "0ec893e0-5785-4de6-99da-4ed124e5296c"

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "azuread_application_pre_authorized" "ms365_app_client_1" {
  application_id       = azuread_application.azure_application.id
  authorized_client_id = "4345a7b9-9a63-4910-a426-35363201d503"

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "azuread_application_pre_authorized" "ms365_app_client_2" {
  application_id       = azuread_application.azure_application.id
  authorized_client_id = "4765445b-32c6-49b0-83e6-1d93765276ca"

  permission_ids = [
    azuread_application_permission_scope.access_as_user.scope_id,
  ]
}

resource "aws_ssm_parameter" "azure_application" {
  name  = module.common.parameter_store_name_azure_application_id
  type  = "String"
  value = azuread_application.azure_application.client_id
  tags  = local.tags
}

resource "azuread_application_password" "azure_application" {
  application_id = azuread_application.azure_application.id
}

resource "aws_ssm_parameter" "azure_application_secret_id" {
  name  = module.common.parameter_store_name_azure_application_secret_id
  type  = "String"
  value = azuread_application_password.azure_application.key_id
  tags  = local.tags
}

resource "aws_ssm_parameter" "azure_application_secret_value" {
  name  = module.common.parameter_store_name_azure_application_secret_value
  type  = "String"
  value = azuread_application_password.azure_application.value
  tags  = local.tags
}
