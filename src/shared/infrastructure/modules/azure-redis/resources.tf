module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}

resource "azurerm_redis_cache" "redis" {
  name                 = "skedular-${module.naming.redis_cache.name}"
  location             = var.region
  resource_group_name  = var.resource_group
  capacity             = 0
  family               = "C"
  sku_name             = "Basic"
  non_ssl_port_enabled = false
  minimum_tls_version  = "1.2"

  redis_configuration {
  }
}

resource "azurerm_redis_firewall_rule" "allowed" {
  for_each = toset(local.allowed_cidrs)

  name                = replace(replace(each.key, ".", "_"), "/", "_")
  redis_cache_name    = azurerm_redis_cache.redis.name
  resource_group_name = var.resource_group

  start_ip = cidrhost(each.key, 0)  # first address in the range
  end_ip   = cidrhost(each.key, -1) # last address in the range
}
