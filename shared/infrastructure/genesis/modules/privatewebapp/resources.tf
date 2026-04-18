module "terraform_state_backend_privatewebapp" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "privatewebapp"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_privatewebapp.tf"
  force_destroy                      = true
}

module "terraform_state_backend_privatewebapp_help" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "privatewebapp-help"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_privatewebapp_help.tf"
  force_destroy                      = true
}
