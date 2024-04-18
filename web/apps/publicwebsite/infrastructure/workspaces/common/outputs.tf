output "tags" {
  description = "Common tags"
  value = {
    domain = "publicwebsite"
  }
}

output "project_name" {
  value = "${var.environment}-publicwebsite"
}

output "cognito_user_pool_client_name" {
  value = "${var.environment}_publicwebsite"
}
