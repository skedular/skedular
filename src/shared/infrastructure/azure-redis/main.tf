module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}


module "azure-redis" {
  source         = "../modules/azure-redis"
  resource_group = data.azurerm_resource_group.existing_rg.name
  environment    = var.environment
  region         = var.region
}
