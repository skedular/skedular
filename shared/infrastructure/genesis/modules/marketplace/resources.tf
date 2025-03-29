module "terraform_state_backend_marketplace_shared" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "marketplace-shared"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_marketplace_shared.tf"
  force_destroy                      = true
}
