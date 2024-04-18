output "tags" {
  description = "Common tags"
  value = {
    domain = "location"
  }
}

output "invitation_to_join_location_new_customer_email_template_name" {
  value = "InvitationToJoinLocationNewCustomer"
}

output "parameter_store_name_invitation_to_join_location_new_customer_email_template_arn" {
  value = "invitation_to_join_location_new_customer_template_arn"
}

output "invitation_to_join_location_existing_customer_email_template_name" {
  value = "InvitationToJoinLocationExistingCustomer"
}

output "parameter_store_name_invitation_to_join_location_existing_customer_email_template_arn" {
  value = "invitation_to_join_location_existing_customer_template_arn"
}
