output "tags" {
  description = "Common tags"
  value = {
    domain = "web"
  }
}

output "parameter_store_name_nextauth_session" {
  value = "nextauth_session"
}

output "parameter_store_name_workos_session" {
  value = "workos_session"
}
