module "common" {
  source = "../common"

  environment = var.environment
}

module "shared_common" {
  source = "../../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

resource "cloudflare_pages_project" "default" {
  account_id        = module.shared_common.cloudflare_account_id
  name              = module.common.project_name
  production_branch = "main"
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
