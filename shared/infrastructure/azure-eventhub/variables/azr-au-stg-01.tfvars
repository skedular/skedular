environment     = "staging"
region          = "Australia East"
subscription_id = "763dfea3-3b46-43a7-9e56-bacef018b4ba"
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
