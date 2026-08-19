module "common" {
  source = "../../workspaces/common"

  environment = var.environment
}

# Get the latest TLS cert from GitHub to authenticate their requests
data "tls_certificate" "github" {
  url = "${local.github_oidc_url}/.well-known/openid-configuration"
}

# Create the OIDC Provider in the AWS Account
resource "aws_iam_openid_connect_provider" "github_actions" {
  url            = local.github_oidc_url
  client_id_list = ["sts.amazonaws.com"]
  # Add any _known_ thumbprints here. If they do change you will need to update.
  # At least this will capture any new ones that might be added.
  thumbprint_list = distinct(
    concat(
      [
        "6938fd4d98bab03faadb97b34396831e3780aea1",
        "1c58a3a8518e8759bf075b76b750d4f2df264fcd",
      ],
      [for certificate in data.tls_certificate.github.certificates : certificate.sha1_fingerprint if certificate.is_ca]
    )
  )
}

data "aws_iam_policy_document" "github_unityhubio_unityhubio_allow" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = [aws_iam_openid_connect_provider.github_actions.arn]
    }
    condition {
      test     = "StringLike"
      variable = "${aws_iam_openid_connect_provider.github_actions.url}:sub"
      values   = ["repo:${module.common.github_repository_unityhubio}:*"]

    }
    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:aud"
      values   = ["sts.amazonaws.com"]

    }
  }
}

resource "aws_iam_role" "github_actions_unityhubio_unityhubio_oidc_assume_role" {
  name               = "github_actions_unityhubio_unityhubio_oidc_assume_role"
  assume_role_policy = data.aws_iam_policy_document.github_unityhubio_unityhubio_allow.json
}

resource "aws_iam_role_policy_attachment" "github_actions_unityhubio_unityhubio_role_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AdministratorAccess"
  role       = aws_iam_role.github_actions_unityhubio_unityhubio_oidc_assume_role.name
}

resource "aws_ssm_parameter" "github_actions_unityhubio_unityhubio_service_account" {
  name  = module.common.parameter_store_name_aws_github_actions_unityhubio_unityhubioassume_role_arn
  type  = "String"
  value = aws_iam_role.github_actions_unityhubio_unityhubio_oidc_assume_role.arn
  tags  = local.tags
}

# Migration to new org/repo: kept in parallel alongside the unityhubio_unityhubio resources above.
data "aws_iam_policy_document" "github_skedular_skedular_allow" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = [aws_iam_openid_connect_provider.github_actions.arn]
    }
    condition {
      test     = "StringLike"
      variable = "${aws_iam_openid_connect_provider.github_actions.url}:sub"
      values   = ["repo:${module.common.github_repository_skedular}:*"]

    }
    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:aud"
      values   = ["sts.amazonaws.com"]

    }
  }
}

resource "aws_iam_role" "github_actions_skedular_skedular_oidc_assume_role" {
  name               = "github_actions_skedular_skedular_oidc_assume_role"
  assume_role_policy = data.aws_iam_policy_document.github_skedular_skedular_allow.json
}

resource "aws_iam_role_policy_attachment" "github_actions_skedular_skedular_role_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AdministratorAccess"
  role       = aws_iam_role.github_actions_skedular_skedular_oidc_assume_role.name
}

resource "aws_ssm_parameter" "github_actions_skedular_skedular_service_account" {
  name  = module.common.parameter_store_name_aws_github_actions_skedular_skedular_assume_role_arn
  type  = "String"
  value = aws_iam_role.github_actions_skedular_skedular_oidc_assume_role.arn
  tags  = local.tags
}
