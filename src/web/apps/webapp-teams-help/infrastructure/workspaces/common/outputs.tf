output "tags" {
  description = "Common tags"
  value = {
    domain = "webapp-teams-help"
  }
}

output "project_name" {
  value = "${var.environment}-webapp-teams-help"
}
