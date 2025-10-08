output "tags" {
  description = "Common tags"
  value = {
    domain = "webapphelp"
  }
}

output "project_name" {
  value = "${var.environment}-webapphelp"
}
