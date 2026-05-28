module "common" {
  source = "../common"

  environment = var.environment
}

resource "random_password" "workossecret" {
  length           = 256
  special          = true
  override_special = "!#$%&*()-_=+[]{}<>:?"
}

resource "aws_ssm_parameter" "workossecret" {
  name  = module.common.parameter_store_name_workos_session
  type  = "String"
  value = random_password.workossecret.result
  tags  = local.tags
}
