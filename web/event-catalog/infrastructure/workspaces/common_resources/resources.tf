module "common" {
  source = "../common"

  environment = var.environment
}

module "web_common" {
  source = "../../../../infrastructure/workspaces/common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

resource "vercel_project" "default" {
  name             = module.common.project_name
  team_id          = local.team_id
  build_command    = "npm run build"
  dev_command      = "npm run start"
  install_command  = "npm install --frozen-lockfile"
  output_directory = "./dist"
  vercel_authentication = {
    deployment_type = "standard_protection"
  }
}

resource "vercel_project_domain" "default" {
  project_id = vercel_project.default.id
  team_id    = local.team_id
  domain     = module.shared_common.eventcatalog_webapp_domain_name_2
}

data "vercel_project_directory" "default" {
  path = "../../.."
}

resource "vercel_deployment" "default" {
  project_id  = vercel_project.default.id
  files       = data.vercel_project_directory.default.files
  path_prefix = data.vercel_project_directory.default.path
  production  = true
  team_id     = local.team_id
}

data "cloudflare_zone" "default" {
  name = module.shared_common.cloudflare_webapp_domain_name_2
}

resource "cloudflare_record" "default" {
  zone_id = data.cloudflare_zone.default.id
  name    = module.shared_common.eventcatalog_webapp_domain_name_2
  content = "cname.vercel-dns.com."
  type    = "CNAME"
  proxied = false
  ttl     = 600

  depends_on = [
    vercel_project_domain.default,
  ]
}
