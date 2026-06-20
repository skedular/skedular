module "terraform_state_backend_webapp_spaces" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "webapp_spaces"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_webapp_spaces.tf"
  force_destroy                      = true
}
