using System.Reflection;
using Enterprise.Shared.Messaging.Nats.Logging;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Logging;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NatsLoggingContractShould
{
    [Fact]
    public void Expose_stable_nats_log_event_identifiers()
    {
        var methods = typeof(NatsLogEvents).GetMethods(BindingFlags.Public | BindingFlags.Static);

        methods.Select(method => method.Name).ShouldContain("ConnectionReady");
        methods.Select(method => method.Name).ShouldContain("ResourceProvisioned");
        methods.Select(method => method.Name).ShouldContain("MessagePublished");
        methods.Select(method => method.Name).ShouldContain("ForwardingFailed");
        methods.Select(method => method.Name).ShouldContain("MessagePublishFailed");
        methods.Select(method => method.Name).ShouldContain("ConsumerStopped");
        methods.Select(method => method.Name).ShouldContain("MessageDelivered");
        methods.Select(method => method.Name).ShouldContain("RetryDelayPending");
        methods.Select(method => method.Name).ShouldContain("MessageAcknowledged");
        methods.Select(method => method.Name).ShouldContain("ConnectionReconnected");
        methods.Select(method => method.Name).ShouldContain("ConsumerShutdown");
        methods.All(method => method.GetParameters().First().ParameterType == typeof(ILogger)).ShouldBeTrue();
    }
}
