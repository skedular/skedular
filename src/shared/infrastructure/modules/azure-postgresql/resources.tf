module "naming" {
  source  = "Azure/naming/azurerm"
  version = "0.4.2"
  prefix  = [var.environment]
}

module "avm-res-network-privatednszone" {
  source      = "Azure/avm-res-network-privatednszone/azurerm"
  version     = "0.4.2"
  domain_name = "${module.naming.dns_zone.name}.postgres.database.azure.com"
  parent_id   = data.azurerm_resource_group.rg.id
  retry = {
    error_message_regex = ["CannotDeleteResource"]
    attempts            = 3
    delay               = "10s"
  }
}

resource "random_password" "adminpassword" {
  length           = 16
  override_special = "_%@"
  special          = true
}

module "postgresql" {
  source                 = "Azure/avm-res-dbforpostgresql-flexibleserver/azurerm"
  version                = "0.1.4"
  location               = var.region
  name                   = module.naming.postgresql_server.name
  resource_group_name    = var.resource_group
  administrator_login    = var.administrator_login
  administrator_password = random_password.adminpassword.result
  auto_grow_enabled      = true
  backup_retention_days  = 7
  create_mode            = "Default"
  delegated_subnet_id    = data.azurerm_subnet.database-subnet.id
  storage_mb             = var.storage_mb

  # databases = {
  #   my_db1 = {
  #     charset   = "UTF8"
  #     collation = "en_US.utf8"
  #     name      = "my_db1"
  #   }
  #   my_db2 = {
  #     charset   = "UTF8"
  #     collation = "en_US.utf8"
  #     name      = "my_db2"
  #   }
  # }

  # Explicitly disable HA; module default is ZoneRedundant but we want single-zone deployment.
  high_availability = null
  # high_availability = {
  #   mode                      = "ZoneRedundant"
  #   standby_availability_zone = 2
  # }

  zone = 1

  server_version      = 17
  sku_name            = var.sku_name
  tags                = local.merged_tags
  private_dns_zone_id = module.avm-res-network-privatednszone.resource_id
}

module "network-security-group" {
  source              = "Azure/avm-res-network-networksecuritygroup/azurerm"
  version             = "0.5.0"
  resource_group_name = var.resource_group
  location            = var.region
  name                = "${module.naming.network_security_group.name}-db"
  security_rules      = local.nsg_rules

  tags = local.merged_tags
}
