module "terraform_state_backend_marketplacewebapp" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "marketplacewebapp"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_marketplacewebapp.tf"
  force_destroy                      = true
}

module "terraform_state_backend_marketplacewebapp_help" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "marketplacewebapp-help"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_marketplacewebapp_help.tf"
  force_destroy                      = true
}
