locals {
  dns_records_dev_tools = ["contabo", "kowl", "dozzle", "crm"]
}

locals {
  dns_records_dev = ["kapp", "dapp", "mapp"]
}

resource "cloudflare_record" "cloudflare_dns_records_dev_tools" {
  count   = local.is_staging ? length(local.dns_records_dev_tools) : 0
  zone_id = data.cloudflare_zone.webapp.id
  name    = element(local.dns_records_dev_tools, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "cloudflare_dns_records_dev" {
  count   = local.is_staging ? length(local.dns_records_dev) : 0
  zone_id = data.cloudflare_zone.webapp.id
  name    = element(local.dns_records_dev, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}
