using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Messaging.Kafka.Exceptions;

public class KafkaSubscriberAlreadyAddedException(ServiceDescriptor serviceDescriptor)
    : Exception($"Kafka subscriber already added: {serviceDescriptor} ");
