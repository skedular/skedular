// using Api.Shared.Clients.Events.UnityHub.Billing.V1.Key;

using Api.Shared.Clients.Events.UnityHub.Billing.V1.Value;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Infrastructure.Configuration.Extensions;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AllInOne;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program : WebHostServiceBase<Program>
{
    public static async Task Main(string[] args)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            cancellationTokenSource.Cancel();
            eventArgs.Cancel = true;
        };

        await EnvironmentHelper.LoadEnvFileAsync(
            Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".env"),
            cancellationToken);

        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
        builder.BuildConfig<Program>(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), args);

        IConfiguration config = builder.Build();
        var kafkaConfiguration = config.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        var kafkaHelper = new KafkaHelper(kafkaConfiguration);
        await Task.WhenAll(
            kafkaHelper.CreateTopicForEventAsync<Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.LocationInternal.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event>(),
            kafkaHelper
                .CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Event>(),
            kafkaHelper
                .CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.OrganizationMember.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.Payment.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Value.Event>(),
            kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event>()
        );

        // await Task.WhenAll(
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Booking.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Customer.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Location.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event>(),
        //
        //     kafkaHelper
        //         .RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.LocationInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.LocationInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Notification.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Organization.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event>(),
        //
        //     kafkaHelper
        //         .RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<
        //             Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Event>(),
        //
        //     kafkaHelper
        //         .RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.OrganizationMember.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<
        //             Api.Shared.Clients.Events.UnityHub.OrganizationMember.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Payment.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Payment.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Team.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event>());

        await Task.WhenAll(
            Billing.Shared.Program.Main(args),
            Booking.Shared.Program.Main(args),
            Customer.Shared.Program.Main(args),
            Location.Shared.Program.Main(args),
            MsTeams.Shared.Program.Main(args),
            Notification.Shared.Program.Main(args),
            Organization.Shared.Program.Main(args),
            Payment.Shared.Program.Main(args),
            Slack.Shared.Program.Main(args),
            Team.Shared.Program.Main(args)
        );

        await Task.WhenAll(
            Gateway.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Billing.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Billing.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Billing.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Booking.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Booking.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Booking.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Customer.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Customer.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Customer.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Location.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Location.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Location.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            MsTeams.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            MsTeams.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            MsTeams.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Notification.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Notification.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Notification.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Organization.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Organization.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Organization.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Payment.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Payment.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Payment.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Slack.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Slack.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Slack.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Team.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Team.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Team.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken)
        );
    }
}
