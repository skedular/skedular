module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}

resource "azurerm_public_ip" "bastion" {
  allocation_method   = "Static"
  location            = var.region
  name                = module.naming.public_ip.name_unique
  resource_group_name = var.resource_group
  sku                 = "Standard"
  tags                = local.merged_tags
  zones               = [1, 2, 3]
}

module "azure_bastion" {
  source              = "Azure/avm-res-network-bastionhost/azurerm"
  version             = "0.8.1"
  location            = var.region
  name                = module.naming.bastion_host.name_unique
  resource_group_name = var.resource_group
  copy_paste_enabled  = true
  enable_telemetry    = true
  file_copy_enabled   = false
  ip_configuration = {
    name                 = "my-ipconfig"
    subnet_id            = data.azurerm_subnet.bastion-subnet.id
    public_ip_address_id = azurerm_public_ip.bastion.id
    create_public_ip     = false
  }
  ip_connect_enabled     = false
  kerberos_enabled       = true
  shareable_link_enabled = false
  sku                    = "Basic"
  tags                   = local.merged_tags
  tunneling_enabled      = true
}

