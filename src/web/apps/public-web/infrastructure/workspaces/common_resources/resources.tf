module "common" {
  source = "../common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

locals {
  public_skedular_url = var.environment == "production" ? "https://skedular.app" : "https://staging.skedular.app"
  slack_url           = var.environment == "production" ? "https://slack.com/oauth/v2/authorize?scope=app_mentions%3Aread%2Cchannels%3Ajoin%2Cchannels%3Amanage%2Cchannels%3Aread%2Cchat%3Awrite%2Cteam%3Aread%2Cusers%3Aread%2Cusers%3Aread.email%2Cusers.profile%3Aread&user_scope=users.profile%3Awrite%2Cusers.profile%3Aread&redirect_uri=https%3A%2F%2Fslackapi.skedular.app%2Fv1%2Fslack%2Fcallback&client_id=118234978193.5578039519830" : "https://slack.com/oauth/v2/authorize?scope=app_mentions%3Aread%2Cchannels%3Ajoin%2Cchannels%3Amanage%2Cchannels%3Aread%2Cchat%3Awrite%2Cteam%3Aread%2Cusers%3Aread%2Cusers%3Aread.email%2Cusers.profile%3Aread&user_scope=users.profile%3Awrite%2Cusers.profile%3Aread&redirect_uri=https%3A%2F%2Fslackapistaging.skedular.app%2Fv1%2Fslack%2Fcallback&client_id=118234978193.5578036232262"

  public_skedular_env_vars = {
    PUBLIC_SKEDULAR_APP_URL           = local.public_skedular_url
    PUBLIC_SKEDULAR_SIGNUP_URL        = local.public_skedular_url
    PUBLIC_SKEDULAR_DEMO_URL          = local.public_skedular_url
    PUBLIC_SKEDULAR_BECOME_HOST_URL   = local.public_skedular_url
    PUBLIC_SKEDULAR_SLACK_INSTALL_URL = local.slack_url
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
  zone_id = module.shared_common.cloudflare_public_website_zone_id
  name    = module.common.domain_name
  content = "${cloudflare_pages_project.default.name}.pages.dev"
  type    = "CNAME"
  proxied = true
  ttl     = 1
}

resource "cloudflare_pages_domain" "default" {
  account_id   = module.shared_common.cloudflare_account_id
  project_name = cloudflare_pages_project.default.name
  name         = module.common.domain_name

  depends_on = [
    cloudflare_dns_record.default,
  ]
}
