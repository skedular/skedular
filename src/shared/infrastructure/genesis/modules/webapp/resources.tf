module "terraform_state_backend_webapp_shared" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "webapp-shared"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_webapp_shared.tf"
  force_destroy                      = true
}

module "terraform_state_backend_webapp" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "webapp"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_webapp.tf"
  force_destroy                      = true
}

module "terraform_state_backend_webapp_eventcatalog" {
  source                             = "cloudposse/tfstate-backend/aws"
  version                            = "1.1.0"
  namespace                          = var.organization_name
  stage                              = var.environment
  name                               = "webapp-eventcatalog"
  attributes                         = ["terraform-state"]
  terraform_backend_config_file_path = "."
  terraform_backend_config_file_name = "backend/backend_webapp_eventcatalog.tf"
  force_destroy                      = true
}
