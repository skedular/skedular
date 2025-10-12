module "common" {
  source = "../common"

  environment = var.environment
}

module "web_common" {
  source = "../../../../../infrastructure/workspaces/common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

data "aws_cognito_user_pools" "user_pool" {
  name = module.shared_common.cognito_user_pool_name
}

locals {
  user_pool_id = tolist(data.aws_cognito_user_pools.user_pool.ids)[0]
  is_staging   = var.environment == "staging"
}

resource "aws_cognito_user_pool_client" "default" {
  name         = module.common.cognito_user_pool_client_name
  user_pool_id = local.user_pool_id

  callback_urls = [
    "http://localhost:15000/api/auth/callback/cognito",
    "https://${module.shared_common.webapp_domain_name}/api/auth/callback/cognito"
  ]

  explicit_auth_flows = [
    "ALLOW_USER_SRP_AUTH",
    "ALLOW_REFRESH_TOKEN_AUTH"
  ]

  generate_secret               = true
  enable_token_revocation       = true
  prevent_user_existence_errors = "ENABLED"

  allowed_oauth_flows_user_pool_client = true
  allowed_oauth_flows                  = ["code"]
  allowed_oauth_scopes                 = ["email", "openid", "profile"]
  supported_identity_providers = [
    module.shared_common.aws_cognito_identity_provider_cognito_provider_name,
    module.shared_common.aws_cognito_identity_provider_google_provider_name
  ]
}

data "aws_ssm_parameter" "workos_secret" {
  name = module.web_common.parameter_store_name_workos_session
}

data "aws_ssm_parameter" "parameter_store_name_azure_application_id" {
  name = module.shared_common.parameter_store_name_azure_application_id
}

data "aws_ssm_parameter" "parameter_store_name_azure_application_secret_value" {
  name = module.shared_common.parameter_store_name_azure_application_secret_value
}

resource "vercel_project" "default" {
  name             = module.common.project_name
  framework        = "nextjs"
  team_id          = local.team_id
  build_command    = "pnpm webapp#build"
  install_command  = "pnpm install --recursive --frozen-lockfile"
  output_directory = "./apps/webapp/.next"
  vercel_authentication = {
    deployment_type = "standard_protection"
  }

  environment = [
    {
      key    = "WORKOS_API_KEY"
      value  = var.workos_api_key
      target = ["development", "preview", "production"]
    },
    {
      key    = "WORKOS_CLIENT_ID"
      value  = module.shared_common.workos_client_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "WORKOS_COOKIE_PASSWORD"
      value  = data.aws_ssm_parameter.workos_secret.value
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_WORKOS_REDIRECT_URI"
      value  = "https://${module.shared_common.webapp_domain_name}/callback"
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_SITE_URL"
      value  = "https://${module.shared_common.webapp_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_MICROANALYTICS_APP_ID"
      value  = module.shared_common.microanalytics_webapp_app_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_LOGROCKET_APP_ID"
      value  = module.shared_common.logrocket_webapp_app_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_SLACK_CLIENT_ID"
      value  = module.shared_common.slack_client_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID"
      value  = var.google_analytics_measurement_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_GOOGLE_TAG_MANAGER_CONTAINER_ID"
      value  = var.google_tag_manager_container_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_GOOGLE_MAPS_API_KEY"
      value  = var.google_map_api_key
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_API_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_APPLICATION_REGISTRATION_ID"
      value  = data.aws_ssm_parameter.parameter_store_name_azure_application_id.value
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_SLACK_REDIRECT_URL"
      value  = "https://slack${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "COGNITO_DOMAIN"
      value  = "https://${module.shared_common.cognito_user_pool_domain}.auth.${module.shared_common.aws_region}.amazoncognito.com"
      target = ["development", "preview", "production"]
    },
    {
      key    = "COGNITO_CLIENT_ID"
      value  = aws_cognito_user_pool_client.default.id
      target = ["development", "preview", "production"]
    },
    {
      key    = "COGNITO_CLIENT_SECRET"
      value  = aws_cognito_user_pool_client.default.client_secret
      target = ["development", "preview", "production"]
    },
    {
      key    = "COGNITO_ISSUER"
      value  = "https://cognito-idp.${module.shared_common.aws_region}.amazonaws.com/${local.user_pool_id}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "GOOGLE_CLIENT_ID"
      value  = var.gcp_web_credentials_client_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "GOOGLE_CLIENT_SECRET"
      value  = var.gcp_web_credentials_client_secret
      target = ["development", "preview", "production"]
    },
    {
      key    = "AZURE_AD_CLIENT_ID"
      value  = data.aws_ssm_parameter.parameter_store_name_azure_application_id.value
      target = ["development", "preview", "production"]
    },
    {
      key    = "AZURE_AD_CLIENT_SECRET"
      value  = data.aws_ssm_parameter.parameter_store_name_azure_application_secret_value.value
      target = ["development", "preview", "production"]
    },
    {
      key    = "GATEWAY_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "ENABLE_EXPERIMENTAL_COREPACK"
      value  = "1"
      target = ["development", "preview", "production"]
    }
  ]
}

resource "vercel_project_domain" "default" {
  project_id = vercel_project.default.id
  team_id    = local.team_id
  domain     = module.shared_common.webapp_domain_name
}

data "vercel_project_directory" "default" {
  path = "../../../../.."
}

resource "vercel_deployment" "default" {
  project_id  = vercel_project.default.id
  files       = data.vercel_project_directory.default.files
  path_prefix = data.vercel_project_directory.default.path
  production  = true
  team_id     = local.team_id
}

resource "cloudflare_dns_record" "default" {
  zone_id = module.shared_common.cloudflare_webapp_zone_id
  name    = local.is_staging ? module.shared_common.webapp_domain_name : "@"
  content = "cname.vercel-dns.com."
  type    = "CNAME"
  proxied = false
  ttl     = 600

  depends_on = [
    vercel_project_domain.default,
  ]
}
