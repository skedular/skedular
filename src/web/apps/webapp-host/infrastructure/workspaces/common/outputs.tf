output "tags" {
  description = "Common tags"
  value = {
    domain = "webapp-host"
  }
}

output "project_name" {
  value = "${var.environment}-webapp-host"
}

output "cognito_user_pool_client_name" {
  value = "${var.environment}_webapp-host"
}
