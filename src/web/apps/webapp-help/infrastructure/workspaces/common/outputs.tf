output "tags" {
  description = "Common tags"
  value = {
    domain = "webapp-help"
  }
}

output "project_name" {
  value = "${var.environment}-webapphelp"
}
