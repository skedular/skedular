module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}

module "azure-aks" {
  source         = "../modules/azure-aks"
  environment    = var.environment
  region         = var.region
  tags           = local.tags
  resource_group = data.azurerm_resource_group.existing_rg.name
  aks_subnet_id  = data.azurerm_subnet.aks_subnet.id
}
