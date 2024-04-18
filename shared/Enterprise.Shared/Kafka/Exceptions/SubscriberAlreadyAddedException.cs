using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Kafka.Exceptions;

public class SubscriberAlreadyAddedException(ServiceDescriptor serviceDescriptor)
    : Exception($"Subscriber already added: {serviceDescriptor} ");
