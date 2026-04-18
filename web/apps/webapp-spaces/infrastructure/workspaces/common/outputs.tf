output "tags" {
  description = "Common tags"
  value = {
    domain = "webapp"
  }
}

output "project_name" {
  value = "${var.environment}-webapp-spaces"
}

output "cognito_user_pool_client_name" {
  value = "${var.environment}_webapp-spaces"
}
