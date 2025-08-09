resource "aws_ses_template" "invitation_to_join_organization_new_customer_email_template" {
  name    = module.common.invitation_to_join_organization_new_customer_email_template_name
  subject = "{{subject}}"
  html    = file("${path.module}/../../../email-templates/invitation-to-join-organization-new-customer/content.html")
  text    = file("${path.module}/../../../email-templates/invitation-to-join-organization-new-customer/content.txt")
}

resource "aws_ses_template" "invitation_to_join_organization_existing_customer_email_template" {
  name    = module.common.invitation_to_join_organization_existing_customer_email_template_name
  subject = "{{subject}}"
  html    = file("${path.module}/../../../email-templates/invitation-to-join-organization-existing-customer/content.html")
  text    = file("${path.module}/../../../email-templates/invitation-to-join-organization-existing-customer/content.txt")
}

resource "aws_ses_template" "invitation_to_join_team_new_customer_email_template" {
  name    = module.common.invitation_to_join_team_new_customer_email_template_name
  subject = "{{subject}}"
  html    = file("${path.module}/../../../email-templates/invitation-to-join-team-new-customer/content.html")
  text    = file("${path.module}/../../../email-templates/invitation-to-join-team-new-customer/content.txt")
}

resource "aws_ses_template" "invitation_to_join_team_existing_customer_email_template" {
  name    = module.common.invitation_to_join_team_existing_customer_email_template_name
  subject = "{{subject}}"
  html    = file("${path.module}/../../../email-templates/invitation-to-join-team-existing-customer/content.html")
  text    = file("${path.module}/../../../email-templates/invitation-to-join-team-existing-customer/content.txt")
}

resource "aws_ses_template" "new_slackworkspace_joined_email_template" {
  name    = module.common.new_slackworkspace_joined_email_template_name
  subject = "{{subject}}"
  html    = file("${path.module}/../../../email-templates/new-slackworkspace-joined/content.html")
  text    = file("${path.module}/../../../email-templates/new-slackworkspace-joined/content.txt")
}
