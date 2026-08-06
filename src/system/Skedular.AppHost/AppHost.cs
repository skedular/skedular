using Enterprise.Shared.Configurations;
using Enterprise.Shared.HealthCheck;
using Microsoft.Extensions.Hosting;
using Projects;

await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", ".env"), CancellationToken.None);
await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".env"), CancellationToken.None);

var builder = DistributedApplication.CreateBuilder(args);
builder.AddNitroComposition();
var redpanda = builder
    .AddContainer("redpanda", "redpandadata/redpanda", "latest")
    .WithEndpoint(targetPort: 19092, name: "kafka")
    .WithHttpEndpoint(targetPort: 8081, name: "schema-registry");

var kafkaEndpoint = redpanda.GetEndpoint("kafka");
var schemaRegistryEndpoint = redpanda.GetEndpoint("schema-registry");

redpanda.WithArgs(async context =>
{
    var kafkaPort = await kafkaEndpoint.Property(EndpointProperty.Port).GetValueAsync(context.CancellationToken);

    foreach (var argument in new object[]
             {
                 "redpanda", "start", "--overprovisioned", "--smp", "1", "--memory", "1G", "--reserve-memory", "0M", "--node-id", "0",
                 "--check=false", "--kafka-addr", "internal://0.0.0.0:29092,external://0.0.0.0:19092", "--advertise-kafka-addr",
                 $"internal://redpanda:29092,external://localhost:{kafkaPort}", "--schema-registry-addr", "0.0.0.0:8081", "--rpc-addr",
                 "redpanda:33145", "--advertise-rpc-addr", "redpanda:33145",
             })
    {
        context.Args.Add(argument);
    }
});

var kafka = builder.AddConnectionString("kafka", ReferenceExpression.Create($"{kafkaEndpoint.Property(EndpointProperty.HostAndPort)}"));
var schemaRegistryUrl = ReferenceExpression.Create($"{schemaRegistryEndpoint}");
var postgres = builder.AddPostgres("postgres").WithImage("postgis/postgis", "18-master");
var temporal = builder.AddTemporalServerContainer("temporal");
#pragma warning disable ASPIRECERTIFICATES001
var redis = builder.AddRedis("redis").WithoutHttpsCertificate();
#pragma warning restore ASPIRECERTIFICATES001

var sharedInfrastructure = builder
    .AddProject<Infrastructure_Shared>("infrastructureshared")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WaitFor(redpanda);

/******************************************************************************************************************************/
var bookingDatabase = postgres.AddDatabase("bookingdb");
var bookingInfrastructure = builder
    .AddProject<Booking_Infrastructure>("bookinginfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(bookingDatabase);

var bookingApi = builder
    .AddProject<Booking_Api>("bookingapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure)
    .WithGraphQLHttpEndpoint();

var bookingProcessors = builder
    .AddProject<Booking_Processors>("bookingprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);

var bookingJobs = builder
    .AddProject<Booking_Jobs>("bookingjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var coreDatabase = postgres.AddDatabase("coredb");
var coreInfrastructure = builder
    .AddProject<Core_Infrastructure>("coreinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(coreDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(coreDatabase);

var coreApi = builder
    .AddProject<Core_Api>("coreapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(coreDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(coreDatabase)
    .WaitForCompletion(coreInfrastructure)
    .WithGraphQLHttpEndpoint();

var coreProcessors = builder
    .AddProject<Core_Processors>("coreprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(coreDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(coreDatabase)
    .WaitForCompletion(coreInfrastructure);

var coreJobs = builder
    .AddProject<Core_Jobs>("corejobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(coreDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(coreDatabase)
    .WaitForCompletion(coreInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var customerDatabase = postgres.AddDatabase("customerdb");
var customerInfrastructure = builder
    .AddProject<Customer_Infrastructure>("customerinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(customerDatabase);

var customerApi = builder
    .AddProject<Customer_Api>("customerapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(customerDatabase)
    .WaitForCompletion(customerInfrastructure)
    .WithGraphQLHttpEndpoint();

var customerProcessors = builder
    .AddProject<Customer_Processors>("customerprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(customerDatabase)
    .WaitForCompletion(customerInfrastructure);

var customerJobs = builder
    .AddProject<Customer_Jobs>("customerjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(customerDatabase)
    .WaitForCompletion(customerInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var locationDatabase = postgres.AddDatabase("locationdb");
var locationInfrastructure = builder
    .AddProject<Location_Infrastructure>("locationinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(locationDatabase);

var locationApi = builder
    .AddProject<Location_Api>("locationapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(locationDatabase)
    .WaitForCompletion(locationInfrastructure)
    .WithGraphQLHttpEndpoint();

var locationProcessors = builder
    .AddProject<Location_Processors>("locationprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(locationDatabase)
    .WaitForCompletion(locationInfrastructure);

var locationJobs = builder
    .AddProject<Location_Jobs>("locationjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(locationDatabase)
    .WaitForCompletion(locationInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var marketplaceDatabase = postgres.AddDatabase("marketplacedb");
var marketplaceInfrastructure = builder
    .AddProject<Marketplace_Infrastructure>("marketplaceinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(marketplaceDatabase);

var marketplaceApi = builder
    .AddProject<Marketplace_Api>("marketplaceapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure)
    .WithGraphQLHttpEndpoint();

var marketplaceProcessors = builder
    .AddProject<Marketplace_Processors>("marketplaceprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure);

var marketplaceJobs = builder
    .AddProject<Marketplace_Jobs>("marketplacejobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var msteamsDatabase = postgres.AddDatabase("msteamsdb");
var msteamsInfrastructure = builder
    .AddProject<MsTeams_Infrastructure>("msteamsinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msteamsDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(msteamsDatabase);

var msteamsApi = builder
    .AddProject<MsTeams_Api>("msteamsapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msteamsDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(msteamsDatabase)
    .WaitForCompletion(msteamsInfrastructure)
    .WithGraphQLHttpEndpoint();

var msteamsProcessors = builder
    .AddProject<MsTeams_Processors>("msteamsprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msteamsDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(msteamsDatabase)
    .WaitForCompletion(msteamsInfrastructure);

var msteamsJobs = builder
    .AddProject<MsTeams_Jobs>("msteamsjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msteamsDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(msteamsDatabase)
    .WaitForCompletion(msteamsInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var organizationDatabase = postgres.AddDatabase("organizationdb");
var organizationInfrastructure = builder
    .AddProject<Organization_Infrastructure>("organizationinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(organizationDatabase);

var organizationApi = builder
    .AddProject<Organization_Api>("organizationapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure)
    .WithGraphQLHttpEndpoint();

var organizationProcessors = builder
    .AddProject<Organization_Processors>("organizationprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure);

var organizationJobs = builder
    .AddProject<Organization_Jobs>("organizationjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var slackDatabase = postgres.AddDatabase("slackdb");
var slackInfrastructure = builder
    .AddProject<Slack_Infrastructure>("slackinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(slackDatabase);

var slackApi = builder
    .AddProject<Slack_Api>("slackapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(slackDatabase)
    .WaitForCompletion(slackInfrastructure)
    .WithGraphQLHttpEndpoint();

var slackProcessors = builder
    .AddProject<Slack_Processors>("slackprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(slackDatabase)
    .WaitForCompletion(slackInfrastructure);

var slackJobs = builder
    .AddProject<Slack_Jobs>("slackjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(slackDatabase)
    .WaitForCompletion(slackInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var teamDatabase = postgres.AddDatabase("teamdb");
var teamInfrastructure = builder
    .AddProject<Team_Infrastructure>("teaminfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(teamDatabase);

var teamApi = builder
    .AddProject<Team_Api>("teamapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(teamDatabase)
    .WaitForCompletion(teamInfrastructure)
    .WithGraphQLHttpEndpoint();

var teamProcessors = builder
    .AddProject<Team_Processors>("teamprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(teamDatabase)
    .WaitForCompletion(teamInfrastructure);

var teamJobs = builder
    .AddProject<Team_Jobs>("teamjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(teamDatabase)
    .WaitForCompletion(teamInfrastructure);
/******************************************************************************************************************************/

_ = builder
    .AddProject<Gateway>("gateway")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithEnvironment("ReverseProxy__Clusters__booking__Destinations__destination1__Address", bookingApi.GetEndpoint("http"))
    .WithEnvironment("ReverseProxy__Clusters__core__Destinations__destination1__Address", coreApi.GetEndpoint("http"))
    .WithEnvironment("ReverseProxy__Clusters__customer__Destinations__destination1__Address", customerApi.GetEndpoint("http"))
    .WithEnvironment("ReverseProxy__Clusters__location__Destinations__destination1__Address", locationApi.GetEndpoint("http"))
    .WithEnvironment("ReverseProxy__Clusters__marketplace__Destinations__destination1__Address", marketplaceApi.GetEndpoint("http"))
    .WithEnvironment("ReverseProxy__Clusters__msteams__Destinations__destination1__Address", msteamsApi.GetEndpoint("http"))
    .WithEnvironment("ReverseProxy__Clusters__organization__Destinations__destination1__Address", organizationApi.GetEndpoint("http"))
    .WithEnvironment("ReverseProxy__Clusters__slack__Destinations__destination1__Address", slackApi.GetEndpoint("http"))
    .WithEnvironment("ReverseProxy__Clusters__team__Destinations__destination1__Address", teamApi.GetEndpoint("http"))
    .WithEnvironment("Subgraphs__booking-api__Url", $"{bookingApi.GetEndpoint("http")}/v1/graphql")
    .WithEnvironment("Subgraphs__core-api__Url", $"{coreApi.GetEndpoint("http")}/v1/graphql")
    .WithEnvironment("Subgraphs__customer-api__Url", $"{customerApi.GetEndpoint("http")}/v1/graphql")
    .WithEnvironment("Subgraphs__location-api__Url", $"{locationApi.GetEndpoint("http")}/v1/graphql")
    .WithEnvironment("Subgraphs__marketplace-api__Url", $"{marketplaceApi.GetEndpoint("http")}/v1/graphql")
    .WithEnvironment("Subgraphs__msteams-api__Url", $"{msteamsApi.GetEndpoint("http")}/v1/graphql")
    .WithEnvironment("Subgraphs__organization-api__Url", $"{organizationApi.GetEndpoint("http")}/v1/graphql")
    .WithEnvironment("Subgraphs__slack-api__Url", $"{slackApi.GetEndpoint("http")}/v1/graphql")
    .WithEnvironment("Subgraphs__team-api__Url", $"{teamApi.GetEndpoint("http")}/v1/graphql")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithNitroComposition(
        new GraphQLCompositionSettings
        {
            EnableGlobalObjectIdentification = true,
        })
    .WithReference(bookingApi)
    .WaitFor(bookingApi)
    .WithReference(coreApi)
    .WaitFor(coreApi)
    .WithReference(customerApi)
    .WaitFor(customerApi)
    .WithReference(locationApi)
    .WaitFor(locationApi)
    .WithReference(marketplaceApi)
    .WaitFor(marketplaceApi)
    .WithReference(msteamsApi)
    .WaitFor(msteamsApi)
    .WithReference(organizationApi)
    .WaitFor(organizationApi)
    .WithReference(slackApi)
    .WaitFor(slackApi)
    .WithReference(teamApi)
    .WaitFor(teamApi);

foreach (var project in new[]
         {
             bookingApi, bookingProcessors, bookingJobs, coreApi, coreProcessors, coreJobs, customerApi, customerProcessors, customerJobs,
             locationApi, locationProcessors, locationJobs, marketplaceApi, marketplaceProcessors, marketplaceJobs, msteamsApi, msteamsProcessors,
             msteamsJobs, organizationApi, organizationProcessors, organizationJobs, slackApi, slackProcessors, slackJobs, teamApi,
             teamProcessors, teamJobs,
         })
{
    ConfigureGrpcUrls(project, bookingApi, coreApi, customerApi, locationApi, marketplaceApi, msteamsApi, organizationApi, slackApi, teamApi);
}

await builder.Build().RunAsync();

static void ConfigureGrpcUrls(
    IResourceBuilder<ProjectResource> project,
    IResourceBuilder<ProjectResource> bookingApi,
    IResourceBuilder<ProjectResource> coreApi,
    IResourceBuilder<ProjectResource> customerApi,
    IResourceBuilder<ProjectResource> locationApi,
    IResourceBuilder<ProjectResource> marketplaceApi,
    IResourceBuilder<ProjectResource> msteamsApi,
    IResourceBuilder<ProjectResource> organizationApi,
    IResourceBuilder<ProjectResource> slackApi,
    IResourceBuilder<ProjectResource> teamApi)
{
    project
        .WithEnvironment("Booking__GrpcUrl", bookingApi.GetEndpoint("http"))
        .WithEnvironment("Core__GrpcUrl", coreApi.GetEndpoint("http"))
        .WithEnvironment("Customer__GrpcUrl", customerApi.GetEndpoint("http"))
        .WithEnvironment("Location__GrpcUrl", locationApi.GetEndpoint("http"))
        .WithEnvironment("Marketplace__GrpcUrl", marketplaceApi.GetEndpoint("http"))
        .WithEnvironment("MsTeams__GrpcUrl", msteamsApi.GetEndpoint("http"))
        .WithEnvironment("Organization__GrpcUrl", organizationApi.GetEndpoint("http"))
        .WithEnvironment("Slack__GrpcUrl", slackApi.GetEndpoint("http"))
        .WithEnvironment("Team__GrpcUrl", teamApi.GetEndpoint("http"));
}
