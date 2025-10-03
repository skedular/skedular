locals {
  prestino_dns_records_dev_tools = ["prestino", "prestinoadminweb", "prestinocustomerweb", "prestinoapi", "prestinokowl", "prestinodozzle", "prestinotemporal", "prestinozipkin", "prestinojaeger", "prestinoredisinsight"]
}

resource "cloudflare_dns_record" "prestino_cloudflare_dns_records_dev_tools" {
  count   = local.is_staging ? length(local.prestino_dns_records_dev_tools) : 0
  zone_id = module.common.cloudflare_webapp_zone_id
  name    = element(local.prestino_dns_records_dev_tools, count.index)
  content = "77.237.241.187"
  type    = "A"
  proxied = false
  ttl     = 600
}
