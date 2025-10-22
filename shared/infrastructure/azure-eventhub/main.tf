module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}


module "azure-eventhub" {
  source         = "../modules/azure-eventhub"
  resource_group = data.azurerm_resource_group.existing_rg.name
  environment    = var.environment
  region         = var.region
  eventhubs      = var.eventhubs
  tags           = local.tags
}
