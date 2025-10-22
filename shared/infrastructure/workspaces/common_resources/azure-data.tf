data "azurerm_resource_group" "existing_rg" {
  count = local.is_staging ? 0 : 1

  name = module.naming.resource_group.name
}
