output "tags" {
  description = "Common tags"
  value = {
    domain = "team"
  }
}

output "invitation_to_join_team_new_customer_email_template_name" {
  value = "InvitationToJoinTeamNewCustomer"
}

output "parameter_store_name_invitation_to_join_team_new_customer_email_template_arn" {
  value = "invitation_to_join_team_new_customer_template_arn"
}

output "invitation_to_join_team_existing_customer_email_template_name" {
  value = "InvitationToJoinTeamExistingCustomer"
}

output "parameter_store_name_invitation_to_join_team_existing_customer_email_template_arn" {
  value = "invitation_to_join_team_existing_customer_template_arn"
}
