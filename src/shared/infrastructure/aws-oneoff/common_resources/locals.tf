locals {
  tags = {
    environment = var.environment
    resource    = "aws-oneoff"
  }

  github_oidc_url = "https://token.actions.githubusercontent.com"
}
