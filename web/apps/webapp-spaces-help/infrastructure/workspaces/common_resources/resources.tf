module "common" {
  source = "../common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

locals {
  is_staging = var.environment == "staging"
}

resource "vercel_project" "default" {
  name             = module.common.project_name
  framework        = "nextjs"
  team_id          = local.team_id
  build_command    = "pnpm webapp-spaces-help#build"
  install_command  = "pnpm install --recursive --frozen-lockfile"
  output_directory = "./apps/webapp-spaces-help/.next"
  vercel_authentication = {
    deployment_type = "standard_protection"
  }
}

resource "vercel_project_domain" "default" {
  project_id = vercel_project.default.id
  team_id    = local.team_id
  domain     = module.shared_common.webapp_spaces_help_domain_name
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
  name    = local.is_staging ? module.shared_common.webapp_spaces_help_domain_name : "help.spaces"
  content = "cname.vercel-dns.com."
  type    = "CNAME"
  proxied = false
  ttl     = 600

  depends_on = [
    vercel_project_domain.default,
  ]
}
