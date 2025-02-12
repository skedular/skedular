output "tags" {
  description = "Common tags"
  value = {
    domain = "webapp-event-catalog"
  }
}

output "project_name" {
  value = "${var.environment}-webapp-event-catalog"
}
