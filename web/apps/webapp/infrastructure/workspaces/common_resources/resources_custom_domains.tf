locals {
  custom_domains_all_environments = toset(["skedulartrial"])
  custom_domains_production_only  = toset(["assembly"])
  custom_domains = (
    local.is_staging
    ? local.custom_domains_all_environments
    : setunion(local.custom_domains_all_environments, local.custom_domains_production_only)
  )
}

resource "vercel_project_domain" "custom_domains" {
  for_each = local.custom_domains

  project_id = vercel_project.default.id
  team_id    = local.team_id
  domain     = "${each.value}.${module.shared_common.webapp_domain_name}"
}

resource "cloudflare_dns_record" "custom_domains" {
  for_each = local.custom_domains

  zone_id = module.shared_common.cloudflare_webapp_zone_id
  name    = "${each.value}.${module.shared_common.webapp_domain_name}"
  content = "cname.vercel-dns.com."
  type    = "CNAME"
  proxied = false
  ttl     = 600

  depends_on = [
    vercel_project_domain.custom_domains,
  ]
}
