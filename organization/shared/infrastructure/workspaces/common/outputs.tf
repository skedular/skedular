output "tags" {
  description = "Common tags"
  value = {
    domain = "organization"
  }
}

output "invitation_to_join_organization_new_customer_email_template_name" {
  value = "InvitationToJoinOrganizationNewCustomer"
}

output "parameter_store_name_invitation_to_join_organization_new_customer_email_template_arn" {
  value = "invitation_to_join_organization_new_customer_template_arn"
}

output "invitation_to_join_organization_existing_customer_email_template_name" {
  value = "InvitationToJoinOrganizationExistingCustomer"
}

output "parameter_store_name_invitation_to_join_organization_existing_customer_email_template_arn" {
  value = "invitation_to_join_organization_existing_customer_template_arn"
}
