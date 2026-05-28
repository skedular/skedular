locals {
  tags = merge(var.tags, { resource_type = "gcp-project" })
}
