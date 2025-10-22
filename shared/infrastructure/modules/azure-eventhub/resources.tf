module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}

resource "azurerm_eventhub_namespace" "this" {
  name                = module.naming.eventhub_namespace.name
  location            = var.region
  resource_group_name = var.resource_group

  sku                      = "Standard"
  capacity                 = 1
  auto_inflate_enabled     = true
  maximum_throughput_units = 4
  tags                     = local.merged_tags
}

resource "azurerm_eventhub" "this" {
  for_each = { for eh in var.eventhubs : eh.name => eh }

  name              = each.value.name
  namespace_id      = azurerm_eventhub_namespace.this.id
  partition_count   = each.value.partition_count
  message_retention = each.value.message_retention
}

resource "azurerm_private_endpoint" "eventhub" {
  for_each            = data.azurerm_subnet.allowed
  name                = "${each.key}-pe"
  location            = var.region
  resource_group_name = var.resource_group
  subnet_id           = each.value.id

  private_service_connection {
    name                           = "${each.key}-pe-conn"
    private_connection_resource_id = azurerm_eventhub_namespace.this.id
    subresource_names              = ["namespace"]
    is_manual_connection           = false
  }
}

resource "azurerm_private_dns_zone" "eventhub_dns" {
  name                = "privatelink.servicebus.windows.net"
  resource_group_name = var.resource_group
}

resource "azurerm_private_dns_zone_virtual_network_link" "link" {
  name                  = "eventhub-dns-link"
  resource_group_name   = var.resource_group
  private_dns_zone_name = azurerm_private_dns_zone.eventhub_dns.name
  virtual_network_id    = data.azurerm_virtual_network.vnet.id
}