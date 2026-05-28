environment     = "staging"
region          = "Australia East"
subscription_id = "edce45e7-5697-4935-bd63-648e9e609083"
eventhubs = [
  {
    name              = "eh-01"
    partition_count   = 4
    message_retention = 1
  },
  {
    name              = "eh-02"
    partition_count   = 4
    message_retention = 1
  }
]
