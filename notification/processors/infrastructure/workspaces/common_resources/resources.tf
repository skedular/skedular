module "shared_common" {
  source = "../../../../../shared/infrastructure/workspaces/common"

  environment = var.environment
}

module "customer_domain_common" {
  source = "../../../../../customer/shared/infrastructure/workspaces/common"

  environment = var.environment
}

module "slack_domain_common" {
  source = "../../../../../slack/shared/infrastructure/workspaces/common"

  environment = var.environment
}

module "organization_domain_common" {
  source = "../../../../../organization/shared/infrastructure/workspaces/common"

  environment = var.environment
}

module "team_domain_common" {
  source = "../../../../../team/shared/infrastructure/workspaces/common"

  environment = var.environment
}

module "common" {
  source = "../common"

  environment = var.environment
}

data "aws_ses_domain_identity" "default" {
  domain = module.shared_common.simple_email_service_domain
}

data "aws_ssm_parameter" "parameter_store_name_new_customer_feedback_through_web_email_template_arn" {
  name = module.customer_domain_common.parameter_store_name_new_customer_feedback_email_template_arn
}

data "aws_ssm_parameter" "parameter_store_name_new_customer_joined_through_web_email_template_arn" {
  name = module.customer_domain_common.parameter_store_name_new_customer_joined_email_template_arn
}

data "aws_ssm_parameter" "parameter_store_name_new_customer_feedback_through_slack_email_template_arn" {
  name = module.slack_domain_common.parameter_store_name_new_customer_feedback_email_template_arn
}

data "aws_ssm_parameter" "parameter_store_name_new_slackworkspace_joined_through_web_email_template_arn" {
  name = module.slack_domain_common.parameter_store_name_new_slackworkspace_joined_email_template_arn
}

data "aws_ssm_parameter" "parameter_store_name_invitation_to_join_organization_new_customer_email_template_arn" {
  name = module.organization_domain_common.parameter_store_name_invitation_to_join_organization_new_customer_email_template_arn
}

data "aws_ssm_parameter" "parameter_store_name_invitation_to_join_organization_existing_customer_email_template_arn" {
  name = module.organization_domain_common.parameter_store_name_invitation_to_join_organization_existing_customer_email_template_arn
}

data "aws_ssm_parameter" "parameter_store_name_invitation_to_join_team_new_customer_email_template_arn" {
  name = module.team_domain_common.parameter_store_name_invitation_to_join_team_new_customer_email_template_arn
}

data "aws_ssm_parameter" "parameter_store_name_invitation_to_join_team_existing_customer_email_template_arn" {
  name = module.team_domain_common.parameter_store_name_invitation_to_join_team_existing_customer_email_template_arn
}

resource "aws_iam_user" "contabo_notification" {
  name = "contabo_notification"
}

data "aws_iam_policy_document" "lambda_ses_policy_document" {
  version = "2012-10-17"

  statement {
    effect = "Allow"
    actions = [
      "ses:SendEmail",
      "ses:SendTemplatedEmail"
    ]
    resources = [
      data.aws_ses_domain_identity.default.arn,
      data.aws_ssm_parameter.parameter_store_name_new_customer_feedback_through_web_email_template_arn.value,
      data.aws_ssm_parameter.parameter_store_name_new_customer_joined_through_web_email_template_arn.value,
      data.aws_ssm_parameter.parameter_store_name_new_customer_feedback_through_slack_email_template_arn.value,
      data.aws_ssm_parameter.parameter_store_name_new_slackworkspace_joined_through_web_email_template_arn.value,
      data.aws_ssm_parameter.parameter_store_name_invitation_to_join_organization_new_customer_email_template_arn.value,
      data.aws_ssm_parameter.parameter_store_name_invitation_to_join_organization_existing_customer_email_template_arn.value,
      data.aws_ssm_parameter.parameter_store_name_invitation_to_join_team_new_customer_email_template_arn.value,
      data.aws_ssm_parameter.parameter_store_name_invitation_to_join_team_existing_customer_email_template_arn.value,
    ]
  }
}

resource "aws_iam_policy" "contabo_lambda_ses_policy" {
  name   = "contabo-notification-processor-ses-policy"
  policy = data.aws_iam_policy_document.lambda_ses_policy_document.json
}

resource "aws_iam_user_policy_attachment" "contabo_notification_policy_attachment" {
  user       = aws_iam_user.contabo_notification.name
  policy_arn = aws_iam_policy.contabo_lambda_ses_policy.arn
}
