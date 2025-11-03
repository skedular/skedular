# module "naming" {
#   source  = "Azure/naming/azurerm"
#   version = "0.4.2"
#   prefix  = [var.environment]
# }

# resource "azurerm_resource_group" "this" {
#   count = local.is_staging ? 0 : 1

#   name     = module.naming.resource_group.name
#   location = var.azure_region
#   tags     = local.tags
# }

# module "azure-networking" {
#   count = local.is_staging ? 0 : 1

#   source         = "../../modules/azure-networking"
#   resource_group = resource.azurerm_resource_group.this[0].name
#   environment    = var.environment
#   region         = var.azure_region
#   tags           = local.tags
# }

# module "azure-redis" {
#   count = local.is_staging ? 0 : 1

#   source         = "../../modules/azure-redis"
#   resource_group = resource.azurerm_resource_group.this[0].name
#   environment    = var.environment
#   region         = var.azure_region
# }

# module "azure-postgresql" {
#   count = local.is_staging ? 0 : 1

#   source         = "../../modules/azure-postgresql"
#   resource_group = resource.azurerm_resource_group.this[0].name
#   environment    = var.environment
#   region         = var.azure_region
#   tags           = local.tags
#   sku_name       = "GP_Standard_D2ads_v5"
#   storage_mb     = 65536
# }

# module "azure-bastion" {
#   count = local.is_staging ? 0 : 1

#   source         = "../../modules/azure-bastion"
#   resource_group = resource.azurerm_resource_group.this[0].name
#   environment    = var.environment
#   region         = var.azure_region
# }

# module "azure-eventhub" {
#   count = local.is_staging ? 0 : 1

#   source         = "../../modules/azure-eventhub"
#   resource_group = resource.azurerm_resource_group.this[0].name
#   environment    = var.environment
#   region         = var.azure_region
#   eventhubs      = var.eventhubs
#   tags           = local.tags
# }
