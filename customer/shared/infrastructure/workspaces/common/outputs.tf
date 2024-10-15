output "tags" {
  description = "Common tags"
  value = {
    domain = "customer"
  }
}

output "new_customer_feedback_email_template_name" {
  value = "NewCustomerFeedbackThroughWeb"
}

output "parameter_store_name_new_customer_feedback_email_template_arn" {
  value = "new_customer_feedback_through_web_template_arn"
}

output "new_customer_joined_email_template_name" {
  value = "NewCustomerJoinedThroughWeb"
}

output "parameter_store_name_new_customer_joined_email_template_arn" {
  value = "new_customer_joined_through_web_template_arn"
}
