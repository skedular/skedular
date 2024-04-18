locals {
  tags = merge(var.tags, { resource_type = "cognito-user-pool" })
}
