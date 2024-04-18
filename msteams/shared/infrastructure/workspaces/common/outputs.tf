output "tags" {
  description = "Common tags"
  value = {
    domain = "msteams"
  }
}

output "parameter_store_name_azure_msteams_application_id" {
  value = "azure_msteams_application_id"
}

output "parameter_store_name_azure_msteams_secret_id" {
  value = "azure_msteams_secret_id"
}

output "parameter_store_name_azure_msteams_secret_value" {
  value = "azure_msteams_secret_value"
}
