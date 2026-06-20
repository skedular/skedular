module "common" {
  source = "../common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

locals {
  public_skedular_url        = var.environment == "production" ? "https://skedular.app" : "https://staging.skedular.app"
  public_skedular_teams_url  = var.environment == "production" ? "https://teams.skedular.app" : "https://teamsstaging.skedular.app"
  public_skedular_spaces_url = var.environment == "production" ? "https://spaces.skedular.app" : "https://spacesstaging.skedular.app"
  public_skedular_host_url   = var.environment == "production" ? "https://host.skedular.app" : "https://hoststaging.skedular.app"
  public_web_site_url        = "https://${module.common.domain_name}"
  slack_url                  = var.environment == "production" ? "https://slack.com/oauth/v2/authorize?scope=app_mentions%3Aread%2Cchannels%3Ajoin%2Cchannels%3Amanage%2Cchannels%3Aread%2Cchat%3Awrite%2Cteam%3Aread%2Cusers%3Aread%2Cusers%3Aread.email%2Cusers.profile%3Aread&user_scope=users.profile%3Awrite%2Cusers.profile%3Aread&redirect_uri=https%3A%2F%2Fslackapi.skedular.app%2Fv1%2Fslack%2Fcallback&client_id=118234978193.5578039519830" : "https://slack.com/oauth/v2/authorize?scope=app_mentions%3Aread%2Cchannels%3Ajoin%2Cchannels%3Amanage%2Cchannels%3Aread%2Cchat%3Awrite%2Cteam%3Aread%2Cusers%3Aread%2Cusers%3Aread.email%2Cusers.profile%3Aread&user_scope=users.profile%3Awrite%2Cusers.profile%3Aread&redirect_uri=https%3A%2F%2Fslackapistaging.skedular.app%2Fv1%2Fslack%2Fcallback&client_id=118234978193.5578036232262"

  public_skedular_env_vars = {
    PUBLIC_WEB_SITE_URL                    = local.public_web_site_url
    PUBLIC_SKEDULAR_APP_URL                = local.public_skedular_url
    PUBLIC_SKEDULAR_SIGNUP_URL             = local.public_skedular_url
    PUBLIC_SKEDULAR_TEAMS_APP_URL          = local.public_skedular_teams_url
    PUBLIC_SKEDULAR_SPACES_APP_URL         = local.public_skedular_spaces_url
    PUBLIC_SKEDULAR_HOST_APP_URL           = local.public_skedular_host_url
    PUBLIC_SKEDULAR_DEMO_URL               = "https://calendly.com/morteza-alizadeh/skedular"
    PUBLIC_SKEDULAR_BECOME_HOST_URL        = local.public_skedular_host_url
    PUBLIC_SKEDULAR_SLACK_INSTALL_URL      = local.slack_url
    PUBLIC_GOOGLE_ANALYTICS_MEASUREMENT_ID = var.google_analytics_measurement_id
    PUBLIC_LOGROCKET_APP_ID                = var.logrocket_app_id
  }
  public_skedular_pages_env_vars = {
    for name, value in local.public_skedular_env_vars : name => {
      type  = "plain_text"
      value = value
    }
  }
}

resource "cloudflare_pages_project" "default" {
  account_id        = module.shared_common.cloudflare_account_id
  name              = module.common.project_name
  production_branch = "main"

  deployment_configs = {
    preview = {
      env_vars = local.public_skedular_pages_env_vars
    }
    production = {
      env_vars = local.public_skedular_pages_env_vars
    }
  }
}

resource "cloudflare_dns_record" "default" {
  for_each = toset(module.common.domain_names)

  zone_id = module.shared_common.cloudflare_public_website_zone_id
  name    = each.value
  content = "${cloudflare_pages_project.default.name}.pages.dev"
  type    = "CNAME"
  proxied = true
  ttl     = 1
}

resource "cloudflare_pages_domain" "default" {
  for_each = toset(module.common.domain_names)

  account_id   = module.shared_common.cloudflare_account_id
  project_name = cloudflare_pages_project.default.name
  name         = each.value

  depends_on = [
    cloudflare_dns_record.default,
  ]
}
