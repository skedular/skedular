output "arn" {
  description = "Simple Email Service identity ARN"
  value       = aws_ses_domain_identity.default.arn
}

output "verification_token" {
  description = "verification token"
  value       = aws_ses_domain_identity.default.verification_token
}

output "dkim_token" {
  description = "DKIM token"
  value       = aws_ses_domain_dkim.default.dkim_tokens
}
