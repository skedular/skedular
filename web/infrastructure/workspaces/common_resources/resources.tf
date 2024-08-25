module "common" {
  source = "../common"

  environment = var.environment
}

# resource "random_password" "nextauthsecret" {
#   length           = 256
#   special          = true
#   override_special = "!#$%&*()-_=+[]{}<>:?"
# }

# resource "aws_ssm_parameter" "nextauthsecret" {
#   name  = module.common.parameter_store_name_nextauth_session
#   type  = "String"
#   value = random_password.nextauthsecret.result
#   tags  = local.tags
# }
