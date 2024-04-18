output "tags" {
  description = "Common tags"
  value = {
    domain = "slack"
  }
}

output "new_customer_feedback_email_template_name" {
  value = "NewCustomerFeedbackThroughSlack"
}

output "parameter_store_name_new_customer_feedback_email_template_arn" {
  value = "new_customer_feedback_through_slack_template_arn"
}
