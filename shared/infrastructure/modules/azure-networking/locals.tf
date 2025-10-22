locals {
  environments = {
    azr-au-ops-01 = {
      vnet_cidr = "10.10.0.0/16"
      notes     = "Operation environment - Centralized management, logging, monitoring"
      subnets   = []
    }
    production = {
      vnet_cidr = "10.20.0.0/16"
      notes     = "Production environment Australia East region - Primary user-facing workloads"
      subnets = {
        ingress-subnet     = { name = "ingress-subnet", address_prefixes = ["10.20.0.0/24"], default_outbound_access_enabled = true, description = "Public IP and Load Balancer" },
        aks-public-subnet  = { name = "aks-public-subnet", address_prefixes = ["10.20.4.0/22"], default_outbound_access_enabled = true, description = "AKS public (customer-facing) workloads" },
        aks-private-subnet = { name = "aks-private-subnet", address_prefixes = ["10.20.8.0/22"], default_outbound_access_enabled = false, nat_gateway = { id = azurerm_nat_gateway.this.id }, route_table = { id = azurerm_route_table.this.id }, description = "AKS private (backend) workloads" },
        aks-system-subnet  = { name = "aks-system-subnet", address_prefixes = ["10.20.12.0/22"], default_outbound_access_enabled = true, description = "K8s system components" },
        database-subnet = {
          name                            = "database-subnet",
          address_prefixes                = ["10.20.16.0/24"],
          default_outbound_access_enabled = false,
          description                     = "PostgreSQL",
          delegations = [{
            name = "Microsoft.DBforPostgreSQL/flexibleServers"
            service_delegation = {
              name = "Microsoft.DBforPostgreSQL/flexibleServers"
            }
          }]
        },
        eventing-subnet = { name = "eventing-subnet", address_prefixes = ["10.20.17.0/24"], default_outbound_access_enabled = false, description = "Kafka brokers" },
        cache-subnet    = { name = "cache-subnet", address_prefixes = ["10.20.18.0/24"], default_outbound_access_enabled = false, description = "Redis nodes" },
        bastion-subnet  = { name = "AzureBastionSubnet", address_prefixes = ["10.20.70.0/27"], default_outbound_access_enabled = false, description = "Bastion or jumpbox access" }
      }
    }
    staging = {
      vnet_cidr = "10.30.0.0/16"
      notes     = "Staging environment Australia East region - Pre-prod environment for release validation"
      subnets = {
        ingress-subnet     = { name = "ingress-subnet", address_prefixes = ["10.30.0.0/24"], default_outbound_access_enabled = true, description = "Public IP and Load Balancer" },
        aks-public-subnet  = { name = "aks-public-subnet", address_prefixes = ["10.30.4.0/22"], default_outbound_access_enabled = true, description = "AKS public (customer-facing) workloads" },
        aks-private-subnet = { name = "aks-private-subnet", address_prefixes = ["10.30.8.0/22"], default_outbound_access_enabled = false, nat_gateway = { id = azurerm_nat_gateway.this.id }, route_table = { id = azurerm_route_table.this.id }, description = "AKS private (backend) workloads" },
        aks-system-subnet  = { name = "aks-system-subnet", address_prefixes = ["10.30.12.0/22"], default_outbound_access_enabled = true, description = "K8s system components" },
        database-subnet = {
          name                            = "database-subnet",
          address_prefixes                = ["10.30.16.0/24"],
          default_outbound_access_enabled = false,
          description                     = "PostgreSQL"
          delegations = [{
            name = "Microsoft.DBforPostgreSQL/flexibleServers"
            service_delegation = {
              name = "Microsoft.DBforPostgreSQL/flexibleServers"
            }
          }]
        },
        eventing-subnet = { name = "eventing-subnet", address_prefixes = ["10.30.17.0/24"], default_outbound_access_enabled = false, description = "Kafka brokers" },
        cache-subnet    = { name = "cache-subnet", address_prefixes = ["10.30.18.0/24"], default_outbound_access_enabled = false, description = "Redis nodes" },
        bastion-subnet  = { name = "AzureBastionSubnet", address_prefixes = ["10.30.70.0/27"], default_outbound_access_enabled = false, description = "Bastion or jumpbox access" }
      }
    }
  }

  vnet_cidr_map = { for env, cfg in local.environments : env => cfg.vnet_cidr }
  subnet_map    = { for env, cfg in local.environments : env => cfg.subnets }

  default_tags = {
    environment = var.environment
    managed_by  = "terraform"
    module      = "azure-networking"
  }
  merged_tags = merge(local.default_tags, var.tags)
}
