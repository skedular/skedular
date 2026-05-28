data "azurerm_resource_group" "existing_rg" {
  name = module.naming.resource_group.name
}
