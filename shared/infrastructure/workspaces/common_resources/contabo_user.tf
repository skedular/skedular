data "aws_ses_domain_identity" "default" {
  domain = module.common.simple_email_service_domain
}

resource "aws_iam_user" "contabo_user" {
  name = "contabo_user"
}

data "aws_iam_policy_document" "contabo_user_policy_document" {
  version = "2012-10-17"

  statement {
    effect = "Allow"
    actions = [
      "ses:SendEmail",
      "ses:SendTemplatedEmail",
      "ses:SendRawEmail",
    ]
    resources = [
      data.aws_ses_domain_identity.default.arn,
      aws_ses_template.invitation_to_join_organization_new_customer_email_template.arn,
      aws_ses_template.invitation_to_join_organization_existing_customer_email_template.arn,
      aws_ses_template.invitation_to_join_team_new_customer_email_template.arn,
      aws_ses_template.invitation_to_join_team_existing_customer_email_template.arn
    ]
  }
}

resource "aws_iam_policy" "contabo_user_ses_policy" {
  name   = "contabo-user-ses-policy"
  policy = data.aws_iam_policy_document.contabo_user_policy_document.json
}

resource "aws_iam_user_policy_attachment" "contabo_user_policy_attachment" {
  user       = aws_iam_user.contabo_user.name
  policy_arn = aws_iam_policy.contabo_user_ses_policy.arn
}

resource "aws_iam_user_policy" "contabo_user_s3_write_access" {
  name = "contabo-user-s3-write-access"
  user = aws_iam_user.contabo_user.name

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "s3:PutObject",
          "s3:PutObjectAcl"
        ]
        Resource = "${aws_s3_bucket.s3_cdn_bucket.arn}/*"
      }
    ]
  })
}

resource "aws_iam_access_key" "contabo_user_access_key" {
  user = aws_iam_user.contabo_user.name
}

resource "aws_ssm_parameter" "contabo_user_access_key_id" {
  name  = module.common.parameter_store_name_contabo_user_access_key_id
  type  = "String"
  value = aws_iam_access_key.contabo_user_access_key.id
  tags  = local.tags
}

resource "aws_ssm_parameter" "contabo_user_secret_access_key" {
  name  = module.common.parameter_store_name_contabo_user_secret_access_key
  type  = "String"
  value = aws_iam_access_key.contabo_user_access_key.secret
  tags  = local.tags
}
