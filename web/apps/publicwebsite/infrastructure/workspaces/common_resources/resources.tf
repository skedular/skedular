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
}

data "cloudflare_zone" "default" {
  name = module.shared_common.cloudflare_domain_name
}

resource "aws_cognito_user_pool_client" "default" {
  name         = module.common.cognito_user_pool_client_name
  user_pool_id = local.user_pool_id

  callback_urls = [
    "http://localhost:15001/api/auth/callback/cognito",
    "https://${module.shared_common.domain_name}/api/auth/callback/cognito",
    "https://mweb.unityhub.io/api/auth/callback/cognito",
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

data "aws_ssm_parameter" "nextauthsecret" {
  name = module.web_common.parameter_store_name_nextauth_session
}

resource "vercel_project" "default" {
  name             = module.common.project_name
  framework        = "nextjs"
  team_id          = local.team_id
  build_command    = "pnpm publicwebsite#build"
  install_command  = "pnpm install --recursive --frozen-lockfile"
  output_directory = "./apps/publicwebsite/.next"
  vercel_authentication = {
    deployment_type = "standard_protection"
  }

  environment = [
    {
      key    = "NEXTAUTH_SECRET"
      value  = data.aws_ssm_parameter.nextauthsecret.value
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXTAUTH_URL"
      value  = "https://${module.shared_common.domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_SITE_URL"
      value  = "https://${module.shared_common.domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_MICROANALYTICS_APP_ID"
      value  = module.shared_common.microanalytics_publicwebsite_app_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "NEXT_PUBLIC_LOGROCKET_APP_ID"
      value  = module.shared_common.logrocket_publicwebsite_app_id
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
      value  = var.gcp_unityhub_web_credentials_client_id
      target = ["development", "preview", "production"]
    },
    {
      key    = "GOOGLE_CLIENT_SECRET"
      value  = var.gcp_unityhub_web_credentials_client_secret
      target = ["development", "preview", "production"]
    },
    {
      key    = "GATEWAY_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "CUSTOMER_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "ORGANIZATION_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "BOOKING_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "NOTIFICATION_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "TEAM_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "LOCATION_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "SLACK_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "PAYMENT_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "BILLING_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "MSTEAMS_ENDPOINT"
      value  = "https://${module.shared_common.api_domain_name}"
      target = ["development", "preview", "production"]
    }
  ]
}

resource "vercel_project_domain" "default" {
  project_id = vercel_project.default.id
  team_id    = local.team_id
  domain     = module.shared_common.domain_name
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

resource "cloudflare_record" "default" {
  zone_id = data.cloudflare_zone.default.id
  name    = var.environment == "production" ? "@" : "staging"
  value   = var.environment == "production" ? "76.76.21.21" : "cname.vercel-dns.com."
  type    = var.environment == "production" ? "A" : "CNAME"
  proxied = false
  ttl     = 600

  depends_on = [
    vercel_project_domain.default,
  ]
}
