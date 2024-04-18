output "tags" {
  description = "Common tags"
  value = {
    domain = "webapp-msteams"
  }
}

output "project_name" {
  value = "${var.environment}-webapp-msteams"
}
