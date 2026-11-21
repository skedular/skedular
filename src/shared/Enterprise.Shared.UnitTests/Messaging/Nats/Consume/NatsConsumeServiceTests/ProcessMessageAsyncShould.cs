using Enterprise.Shared.Hosting;
using Enterprise.Shared.Messaging.Nats.Configuration;
using Enterprise.Shared.Messaging.Nats.Consume;
using Enterprise.Shared.Messaging.Nats.Produce;
using Enterprise.Shared.Messaging.Nats.Provisioning;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.JetStream;
using TestHandler = Enterprise.Shared.Messaging.Nats.Consume.INatsMessageHandler<Google.Protobuf.WellKnownTypes.StringValue,
    Google.Protobuf.WellKnownTypes.StringValue>;
using TestService = Enterprise.Shared.Messaging.Nats.Consume.NatsConsumeService<Google.Protobuf.WellKnownTypes.StringValue,
    Google.Protobuf.WellKnownTypes.StringValue>;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Consume.NatsConsumeServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProcessMessageAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Acknowledge_only_after_successful_handling(
        INatsConnection connection,
        INatsJetStreamProvisioner provisioner,
        TestHandler handler,
        INatsPublisherForwarder forwarder,
        TimeProvider timeProvider,
        IHostApplicationLifetimeWrapper lifetime,
        ILogger<TestService> logger,
        INatsJSMsg<byte[]> message,
        byte[] payload)
    {
        A.CallTo(() => message.Subject).Returns("events.main");
        A.CallTo(() => message.Data).Returns(payload);
        A.CallTo(() => message.Headers).Returns(null);
        using var sut = new TestConsumer(new NatsConfiguration(), connection, provisioner, "events.main", handler, forwarder,
            [], true, timeProvider, lifetime, logger);

        await sut.ProcessAsync(message, CancellationToken.None);

        A.CallTo(() => handler.HandleMessageAsync(
                A<NatsMessage>.That.Matches(value => value.Subject == "events.main" && value.Payload.ToArray().SequenceEqual(payload)),
                CancellationToken.None))
            .MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => message.AckAsync(null, CancellationToken.None)).MustHaveHappenedOnceExactly());
        A.CallTo(() => message.NakAsync(A<AckOpts?>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Advance_retries_instead_of_looping_back_to_the_first_retry(
        INatsConnection connection,
        INatsJetStreamProvisioner provisioner,
        TestHandler handler,
        INatsPublisherForwarder forwarder,
        TimeProvider timeProvider,
        IHostApplicationLifetimeWrapper lifetime,
        ILogger<TestService> logger,
        INatsJSMsg<byte[]> message,
        byte[] payload)
    {
        A.CallTo(() => message.Data).Returns(payload);
        A.CallTo(() => message.Headers).Returns(null);
        A.CallTo(() => handler.HandleMessageAsync(A<NatsMessage>._, A<CancellationToken>._))
            .Throws(new InvalidOperationException("Handler failed."));
        var retries = NatsRetrySubjectSetting.Create("events.main", 2);

        foreach (var (subject, expectedNext) in new[]
                 {
                     ("events.main", "events.main.retry.0"), ("events.main.retry.0", "events.main.retry.1"),
                     ("events.main.retry.1", "events.main.dead"),
                 })
        {
            A.CallTo(() => message.Subject).Returns(subject);
            using var sut = new TestConsumer(new NatsConfiguration(), connection, provisioner, subject, handler, forwarder,
                retries, true, timeProvider, lifetime, logger);

            await sut.ProcessAsync(message, CancellationToken.None);

            A.CallTo(() => forwarder.PublishForwardedAsync(
                    expectedNext, A<ReadOnlyMemory<byte>>.That.Matches(value => value.ToArray().SequenceEqual(payload)),
                    A<IReadOnlyDictionary<string, string>>.That.Matches(headers => headers.ContainsKey(NatsMessage.RetryTimestampHeader)),
                    CancellationToken.None))
                .MustHaveHappenedOnceExactly();
        }

        A.CallTo(() => message.AckAsync(null, CancellationToken.None)).MustHaveHappened(3, Times.Exactly);
        A.CallTo(() => message.NakAsync(A<AckOpts?>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Redeliver_a_failed_forward_without_acknowledging(
        INatsConnection connection,
        INatsJetStreamProvisioner provisioner,
        TestHandler handler,
        INatsPublisherForwarder forwarder,
        TimeProvider timeProvider,
        IHostApplicationLifetimeWrapper lifetime,
        ILogger<TestService> logger,
        INatsJSMsg<byte[]> message)
    {
        var configuration = new NatsConfiguration
        {
            RedeliveryDelay = TimeSpan.FromSeconds(7),
        };
        A.CallTo(() => message.Subject).Returns("events.main");
        A.CallTo(() => message.Headers).Returns(null);
        A.CallTo(() => handler.HandleMessageAsync(A<NatsMessage>._, A<CancellationToken>._))
            .Throws(new InvalidOperationException("Handler failed."));
        A.CallTo(() => forwarder.PublishForwardedAsync(
                A<string>._, A<ReadOnlyMemory<byte>>._, A<IReadOnlyDictionary<string, string>>._, A<CancellationToken>._))
            .Throws(new InvalidOperationException("Publish failed."));
        using var sut = new TestConsumer(configuration, connection, provisioner, "events.main", handler, forwarder,
            NatsRetrySubjectSetting.Create("events.main", 1), true, timeProvider, lifetime, logger);

        await sut.ProcessAsync(message, CancellationToken.None);

        A.CallTo(() => message.NakAsync(
                A<AckOpts?>.That.Matches(options => options.HasValue && options.Value.NakDelay == configuration.RedeliveryDelay),
                CancellationToken.None))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => message.AckAsync(A<AckOpts?>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_dead_letter_and_non_reliable_failures_local(
        INatsConnection connection,
        INatsJetStreamProvisioner provisioner,
        TestHandler handler,
        INatsPublisherForwarder forwarder,
        TimeProvider timeProvider,
        IHostApplicationLifetimeWrapper lifetime,
        ILogger<TestService> logger,
        INatsJSMsg<byte[]> message)
    {
        A.CallTo(() => message.Headers).Returns(null);
        A.CallTo(() => handler.HandleMessageAsync(A<NatsMessage>._, A<CancellationToken>._))
            .Throws(new InvalidOperationException("Handler failed."));
        foreach (var (subject, reliable) in new[] { ("events.main.dead", true), ("events.main", false) })
        {
            A.CallTo(() => message.Subject).Returns(subject);
            using var sut = new TestConsumer(new NatsConfiguration(), connection, provisioner, subject, handler, forwarder,
                NatsRetrySubjectSetting.Create("events.main", 1), reliable, timeProvider, lifetime, logger);

            await sut.ProcessAsync(message, CancellationToken.None);
        }

        A.CallTo(() => message.NakAsync(A<AckOpts?>._, CancellationToken.None)).MustHaveHappenedTwiceExactly();
        A.CallTo(() => message.AckAsync(A<AckOpts?>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => forwarder.PublishForwardedAsync(
                A<string>._, A<ReadOnlyMemory<byte>>._, A<IReadOnlyDictionary<string, string>>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => lifetime.StopApplication()).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Send_directly_to_dead_letter_when_no_retries_are_configured(
        INatsConnection connection,
        INatsJetStreamProvisioner provisioner,
        TestHandler handler,
        INatsPublisherForwarder forwarder,
        TimeProvider timeProvider,
        IHostApplicationLifetimeWrapper lifetime,
        ILogger<TestService> logger,
        INatsJSMsg<byte[]> message)
    {
        A.CallTo(() => message.Subject).Returns("events.main");
        A.CallTo(() => message.Headers).Returns(null);
        A.CallTo(() => handler.HandleMessageAsync(A<NatsMessage>._, A<CancellationToken>._))
            .Throws(new InvalidOperationException("Handler failed."));
        using var sut = new TestConsumer(new NatsConfiguration(), connection, provisioner, "events.main", handler, forwarder,
            [], true, timeProvider, lifetime, logger);

        await sut.ProcessAsync(message, CancellationToken.None);

        A.CallTo(() => forwarder.PublishForwardedAsync(
                "events.main.dead", A<ReadOnlyMemory<byte>>._, A<IReadOnlyDictionary<string, string>>._, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => message.AckAsync(null, CancellationToken.None)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Propagate_shutdown_cancellation_without_forwarding(
        INatsConnection connection,
        INatsJetStreamProvisioner provisioner,
        TestHandler handler,
        INatsPublisherForwarder forwarder,
        TimeProvider timeProvider,
        IHostApplicationLifetimeWrapper lifetime,
        ILogger<TestService> logger,
        INatsJSMsg<byte[]> message)
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        A.CallTo(() => message.Subject).Returns("events.main");
        A.CallTo(() => message.Headers).Returns(null);
        A.CallTo(() => handler.HandleMessageAsync(A<NatsMessage>._, cancellation.Token))
            .Throws(new OperationCanceledException(cancellation.Token));
        using var sut = new TestConsumer(new NatsConfiguration(), connection, provisioner, "events.main", handler, forwarder,
            [], true, timeProvider, lifetime, logger);

        await Should.ThrowAsync<OperationCanceledException>(() => sut.ProcessAsync(message, cancellation.Token));

        A.CallTo(() => message.AckAsync(A<AckOpts?>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => message.NakAsync(A<AckOpts?>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => forwarder.PublishForwardedAsync(
                A<string>._, A<ReadOnlyMemory<byte>>._, A<IReadOnlyDictionary<string, string>>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    private sealed class TestConsumer(
        NatsConfiguration configuration,
        INatsConnection connection,
        INatsJetStreamProvisioner provisioner,
        string subject,
        TestHandler handler,
        INatsPublisherForwarder forwarder,
        IReadOnlyList<NatsRetrySubjectSetting> retries,
        bool reliable,
        TimeProvider timeProvider,
        IHostApplicationLifetimeWrapper lifetime,
        ILogger<TestService> logger)
        : TestService(
            configuration, connection, provisioner, "events", "consumer", subject, handler, forwarder,
            retries, "events.main.dead", reliable, timeProvider, lifetime, logger)
    {
        public Task ProcessAsync(INatsJSMsg<byte[]> message, CancellationToken cancellationToken) =>
            ProcessMessageAsync(message, cancellationToken);
    }
}
