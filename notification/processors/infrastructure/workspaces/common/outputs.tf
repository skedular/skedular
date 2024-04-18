output "lambda_name_notification_v1_event" {
  value = "notification_not_v1_event"
}

output "lambda_name_customer_v1_event" {
  value = "notification_cus_v1_event"
}

output "lambda_name_organization_v1_event" {
  value = "notification_org_v1_event"
}

output "lambda_name_location_v1_event" {
  value = "notification_loc_v1_event"
}

output "lambda_name_team_v1_event" {
  value = "notification_tea_v1_event"
}

output "database_user" {
  value = "notification_processor"
}

output "consumer_group" {
  value = "urn::unityhub::notification::notification-processor"
}
