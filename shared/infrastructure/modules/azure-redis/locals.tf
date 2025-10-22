locals {
  default_tags = {
    environment = var.environment
    managed_by  = "terraform"
    module      = "azure-redis"
  }
  merged_tags = merge(local.default_tags, var.tags)

  # Flatten all prefixes to a list of CIDR strings
  allowed_cidrs = [
    for s in data.azurerm_subnet.allowed :
    # Some VNets use multiple prefixes; pick the first or join if needed
    try(s.address_prefixes[0], s.address_prefix)
  ]
}
