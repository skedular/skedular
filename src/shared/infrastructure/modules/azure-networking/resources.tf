module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}

#Creating a Route Table with a unique name in the specified location.
resource "azurerm_route_table" "this" {
  location            = var.region
  name                = module.naming.route_table.name_unique
  resource_group_name = var.resource_group
}

# Creating a DDoS Protection Plan in the specified location.
# resource "azurerm_network_ddos_protection_plan" "this" {
#   location            = var.region
#   name                = module.naming.network_ddos_protection_plan.name_unique
#   resource_group_name = var.resource_group
# }

#Creating a NAT Gateway in the specified location.
resource "azurerm_nat_gateway" "this" {
  location            = var.region
  name                = module.naming.nat_gateway.name_unique
  resource_group_name = var.resource_group
}

module "avm-res-network-virtualnetwork" {
  source              = "Azure/avm-res-network-virtualnetwork/azurerm"
  version             = "0.9.3"
  address_space       = [local.vnet_cidr_map[var.environment]]
  location            = var.region
  name                = module.naming.virtual_network.name
  resource_group_name = var.resource_group
  subnets             = local.subnet_map[var.environment]
}

resource "azurerm_key_vault" "vault" {
  name                = "skedular-${module.naming.key_vault.name}"
  location            = var.region
  resource_group_name = var.resource_group
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions    = var.key_permissions
    secret_permissions = var.secret_permissions
  }
}
