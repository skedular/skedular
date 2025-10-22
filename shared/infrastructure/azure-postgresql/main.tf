module "azure-postgresql" {
  source         = "../modules/azure-postgresql"
  resource_group = data.azurerm_resource_group.existing_rg.name
  environment    = var.environment
  region         = var.region
  tags           = local.tags
  sku_name       = "GP_Standard_D2s_v3"
  storage_mb     = 32768
}

module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}
