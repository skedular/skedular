# module "azure-postgresql" {
#   count = local.is_staging ? 0 : 1

#   source         = "../../modules/azure-postgresql"
#   resource_group = data.azurerm_resource_group.existing_rg[0].name
#   environment    = var.environment
#   region         = var.azure_region
#   tags           = local.tags
#   sku_name       = "GP_Standard_D2s_v3"
#   storage_mb     = 32768
# }
