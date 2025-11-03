variable "environment" {
  type        = string
  description = "environment name"
}

variable "gcp_web_credentials_client_id" {
  type        = string
  description = "GCP web credentials client Id"
}

variable "gcp_web_credentials_client_secret" {
  type        = string
  description = "GCP web credentials client Secret"
}

variable "log_retention" {
  description = "retention in days"
  type        = number
  default     = 7
}

variable "azure_region" {
  description = "Azure region for resource deployment."
  type        = string
  default     = null
}

variable "eventhubs" {
  description = "List of event hubs with config"
  type = list(object({
    name              = string
    partition_count   = number
    message_retention = number
  }))
  default = [
    {
      name              = "prod.booking.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.booking.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.booking.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.booking.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.booking.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.booking.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },





    {
      name              = "prod.booking.internal"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.booking.internal.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.booking.internal.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.booking.internal"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.booking.internal.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.booking.internal.deadletter"
      partition_count   = 3
      message_retention = 7
    },






    {
      name              = "prod.customer.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.customer.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.customer.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.customer.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.customer.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.customer.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },





    {
      name              = "prod.location.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.location.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.location.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.location.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.location.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.location.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },





    {
      name              = "prod.marketplace.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.marketplace.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.marketplace.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.marketplace.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.marketplace.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.marketplace.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },


    {
      name              = "prod.organization.member.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.organization.member.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.organization.member.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.member.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.member.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.member.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },




    {
      name              = "prod.organization.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.organization.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.organization.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },





    {
      name              = "prod.organization.internal"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.organization.internal.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.organization.internal.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.internal"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.internal.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.organization.internal.deadletter"
      partition_count   = 3
      message_retention = 7
    },



    {
      name              = "prod.team.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.team.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "prod.team.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.team.v1.event"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.team.v1.event.retry.0"
      partition_count   = 3
      message_retention = 7
    },
    {
      name              = "staging.team.v1.event.deadletter"
      partition_count   = 3
      message_retention = 7
    },
  ]
}
