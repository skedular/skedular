output "tags" {
  description = "Common tags"
  value = {
    domain = "webapp-spaces-help"
  }
}

output "project_name" {
  value = "${var.environment}-webapp-spaces-help"
}
