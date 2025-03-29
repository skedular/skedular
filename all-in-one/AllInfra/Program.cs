// using Api.Shared.Clients.Events.Skedular.Billing.V1.Key;

using Api.Shared.Clients.Events.Skedular.Billing.V1.Value;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Microsoft.Extensions.Configuration;

namespace AllInfra;

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
        await kafkaHelper.CreateTopicForEventAsync<Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.BillingInternal.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event>();
        await kafkaHelper
            .CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.MsTeamsInternal.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event>();
        await kafkaHelper
            .CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event>();
        await kafkaHelper
            .CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value.Event>();
        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event>();

        // await Task.WhenAll([
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BillingInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BillingInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Booking.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Customer.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Location.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event>(),
        //
        //     kafkaHelper
        //         .RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.MsTeamsInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.MsTeamsInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Notification.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Organization.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event>(),
        //
        //     kafkaHelper
        //         .RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<
        //             Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event>(),
        //
        //     kafkaHelper
        //         .RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<
        //             Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Payment.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Key.Key>(),
        //     kafkaHelper
        //         .RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value.Event>(),
        //
        //     kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Team.V1.Key.Key>(),
        //     kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event>()
        // ]);

        await Billing.Shared.Program.Main(args);
        await Booking.Shared.Program.Main(args);
        await Customer.Shared.Program.Main(args);
        await Location.Shared.Program.Main(args);
        await Marketplace.Shared.Program.Main(args);
        await MsTeams.Shared.Program.Main(args);
        await Notification.Shared.Program.Main(args);
        await Organization.Shared.Program.Main(args);
        await Payment.Shared.Program.Main(args);
        await Slack.Shared.Program.Main(args);
        await Team.Shared.Program.Main(args);
    }
}
