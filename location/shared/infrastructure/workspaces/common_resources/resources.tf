module "common" {
  source = "../common"

  environment = var.environment
}

resource "aws_ses_template" "invitation_to_join_location_new_customer_email_template" {
  name    = module.common.invitation_to_join_location_new_customer_email_template_name
  subject = "{{subject}}"
  html    = file("${path.module}/../../../email-templates/invitation-to-join-location-new-customer/content.html")
  text    = file("${path.module}/../../../email-templates/invitation-to-join-location-new-customer/content.txt")
}

resource "aws_ssm_parameter" "invitation_to_join_location_new_customer_email_template" {
  name  = module.common.parameter_store_name_invitation_to_join_location_new_customer_email_template_arn
  type  = "String"
  value = aws_ses_template.invitation_to_join_location_new_customer_email_template.arn
  tags  = merge(local.tags, module.common.tags)
}

resource "aws_ses_template" "invitation_to_join_location_existing_customer_email_template" {
  name    = module.common.invitation_to_join_location_existing_customer_email_template_name
  subject = "{{subject}}"
  html    = file("${path.module}/../../../email-templates/invitation-to-join-location-existing-customer/content.html")
  text    = file("${path.module}/../../../email-templates/invitation-to-join-location-existing-customer/content.txt")
}

resource "aws_ssm_parameter" "invitation_to_join_location_existing_customer_email_template" {
  name  = module.common.parameter_store_name_invitation_to_join_location_existing_customer_email_template_arn
  type  = "String"
  value = aws_ses_template.invitation_to_join_location_existing_customer_email_template.arn
  tags  = merge(local.tags, module.common.tags)
}
