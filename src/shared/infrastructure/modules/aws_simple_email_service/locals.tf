locals {
  tags = merge(var.tags, { resource_type = "simple-email-service" })
}
