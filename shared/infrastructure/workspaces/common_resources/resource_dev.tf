locals {
  azure_app_display_name_dev = "UnityHub-dev"
  azure_app_description_dev  = "UnityHub-dev"
  dns_records_dev            = ["aweb", "aapp", "amsteams", "dweb", "dapp", "dmsteams", "mweb", "mapp", "mmsteams", "yweb", "yapp", "ymsteams", "contabo"]
}

resource "cloudflare_record" "cloudflare_dns_records_dev_1" {
  count   = local.is_staging ? length(local.dns_records_dev) : 0
  zone_id = data.cloudflare_zone.webapp_1.id
  name    = element(local.dns_records_dev, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}

resource "cloudflare_record" "cloudflare_dns_records_dev_2" {
  count   = local.is_staging ? length(local.dns_records_dev) : 0
  zone_id = data.cloudflare_zone.webapp_2.id
  name    = element(local.dns_records_dev, count.index)
  content = "31.220.100.177"
  type    = "A"
  proxied = false
  ttl     = 600
}
