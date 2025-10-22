locals {
  default_tags = {
    environment = var.environment
    managed_by  = "terraform"
    module      = "azure-bastion"
  }
  merged_tags = merge(local.default_tags, var.tags)
}
