locals {
  default_tags = {
    environment = var.environment
    managed_by  = "terraform"
    module      = "azure-redis"
  }
  merged_tags = merge(local.default_tags, var.tags)
}
