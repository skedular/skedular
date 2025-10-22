locals {
  default_tags = {
    environment = var.environment
    managed_by  = "terraform"
    module      = "azure-postgresql"
  }
  merged_tags = merge(local.default_tags, var.tags)
  nsg_rules = {
    "rule01" = {
      name                       = "AllowPostgreSQLInBoundAks"
      priority                   = 100
      direction                  = "Inbound"
      access                     = "Allow"
      protocol                   = "Tcp"
      source_port_range          = "*"
      destination_port_range     = "5432"
      source_address_prefix      = data.azurerm_subnet.aks-private-subnet.address_prefixes[0]
      destination_address_prefix = "*"
    },
    "rule02" = {
      name                       = "AllowPostgreSQLInBoundBastion"
      priority                   = 101
      direction                  = "Inbound"
      access                     = "Allow"
      protocol                   = "Tcp"
      source_port_range          = "*"
      destination_port_range     = "5432"
      source_address_prefix      = data.azurerm_subnet.bastion-subnet.address_prefixes[0]
      destination_address_prefix = "*"
    },
    "rule03" = {
      name                       = "DenyAllInBound"
      priority                   = 4096
      direction                  = "Inbound"
      access                     = "Deny"
      protocol                   = "*"
      source_port_range          = "*"
      destination_port_range     = "*"
      source_address_prefix      = "*"
      destination_address_prefix = "*"
    },
    "rule04" = {
      name                       = "AllowAllOutBound"
      priority                   = 100
      direction                  = "Outbound"
      access                     = "Allow"
      protocol                   = "*"
      source_port_range          = "*"
      destination_port_range     = "*"
      source_address_prefix      = "*"
      destination_address_prefix = "*"
    }
  }
}
