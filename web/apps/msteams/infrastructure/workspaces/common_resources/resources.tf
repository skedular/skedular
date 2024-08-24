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

data "cloudflare_zone" "default" {
  name = module.shared_common.cloudflare_domain_name
}

data "aws_ssm_parameter" "parameter_store_name_azure_application_id" {
  name = module.shared_common.parameter_store_name_azure_application_id
}

resource "vercel_project" "default" {
  name             = module.common.project_name
  team_id          = local.team_id
  build_command    = "pnpm msteams#build"
  install_command  = "pnpm install --recursive --frozen-lockfile"
  output_directory = "./apps/msteams/build"
  vercel_authentication = {
    deployment_type = "standard_protection"
  }

  environment = [
    {
      key    = "REACT_APP_BASE_URL"
      value  = "https://${module.shared_common.msteams_webapp_domain_name}"
      target = ["development", "preview", "production"]
    },
    {
      key    = "REACT_APP_APPLICATION_REGISTRATION_ID"
      value  = data.aws_ssm_parameter.parameter_store_name_azure_application_id.value
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
    }
  ]
}

resource "vercel_project_domain" "default" {
  project_id = vercel_project.default.id
  team_id    = local.team_id
  domain     = module.shared_common.msteams_webapp_domain_name
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
  name    = module.shared_common.msteams_webapp_domain_name
  content = "cname.vercel-dns.com."
  type    = "CNAME"
  proxied = false
  ttl     = 600

  depends_on = [
    vercel_project_domain.default,
  ]
}
