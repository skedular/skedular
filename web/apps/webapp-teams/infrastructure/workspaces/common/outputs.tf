output "tags" {
  description = "Common tags"
  value = {
    domain = "webapp-teams"
  }
}

output "project_name" {
  value = "${var.environment}-webapp-teams"
}

output "cognito_user_pool_client_name" {
  value = "${var.environment}_webapp-teams"
}
