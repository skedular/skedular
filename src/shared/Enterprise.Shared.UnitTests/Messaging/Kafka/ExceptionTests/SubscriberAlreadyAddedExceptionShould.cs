using Enterprise.Shared.Messaging.Kafka.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.UnitTests.Messaging.Kafka.ExceptionTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SubscriberAlreadyAddedExceptionShould
{
    [Fact]
    public void Include_service_descriptor_in_message()
    {
        var descriptor = ServiceDescriptor.Singleton<object, object>();

        var ex = new KafkaSubscriberAlreadyAddedException(descriptor);

        ex.Message.ShouldContain(descriptor.ToString());
    }
}
