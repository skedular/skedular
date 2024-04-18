module "common" {
  source = "../common"

  environment = var.environment
}

resource "aws_ses_template" "new_customer_feedback_email_template" {
  name    = module.common.new_customer_feedback_email_template_name
  subject = "{{subject}}"
  html    = file("${path.module}/../../../email-templates/new-customer-feedback/content.html")
  text    = file("${path.module}/../../../email-templates/new-customer-feedback/content.txt")
}

resource "aws_ssm_parameter" "new_customer_feedback_email_template" {
  name  = module.common.parameter_store_name_new_customer_feedback_email_template_arn
  type  = "String"
  value = aws_ses_template.new_customer_feedback_email_template.arn
  tags  = merge(local.tags, module.common.tags)
}
