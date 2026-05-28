data "azurerm_subnet" "bastion-subnet" {
  name                 = "AzureBastionSubnet"
  virtual_network_name = module.naming.virtual_network.name
  resource_group_name  = var.resource_group
}