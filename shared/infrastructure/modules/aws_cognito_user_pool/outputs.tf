output "user_pool_id" {
  description = "user pool Id"
  value       = aws_cognito_user_pool.default.id
}

output "user_pool_name" {
  description = "user pool name"
  value       = aws_cognito_user_pool.default.name
}
