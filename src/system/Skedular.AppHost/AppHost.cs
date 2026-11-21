using Aspire.Hosting.Dotnet;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.HealthCheck;
using Microsoft.Extensions.Hosting;

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
    .AddDotnetProject("infrastructureshared", "../../shared/Infrastructure.Shared/Infrastructure.Shared.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WaitFor(redpanda);

/******************************************************************************************************************************/
var bookingDatabase = postgres.AddDatabase("bookingdb");
var bookingInfrastructure = builder
    .AddDotnetProject("bookinginfrastructure", "../../booking/shared/Booking.Infrastructure/Booking.Infrastructure.csproj")
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
    .AddDotnetProject("bookingapi", "../../booking/apis/Booking.Api/Booking.Api.csproj")
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
    .AddDotnetProject("bookingprocessors", "../../booking/processors/Booking.Processors/Booking.Processors.csproj")
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
    .AddDotnetProject("bookingjobs", "../../booking/jobs/Booking.Jobs/Booking.Jobs.csproj")
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
    .AddDotnetProject("coreinfrastructure", "../../core/shared/Core.Infrastructure/Core.Infrastructure.csproj")
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
    .AddDotnetProject("coreapi", "../../core/apis/Core.Api/Core.Api.csproj")
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
    .AddDotnetProject("coreprocessors", "../../core/processors/Core.Processors/Core.Processors.csproj")
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
    .AddDotnetProject("corejobs", "../../core/jobs/Core.Jobs/Core.Jobs.csproj")
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
    .AddDotnetProject("customerinfrastructure", "../../customer/shared/Customer.Infrastructure/Customer.Infrastructure.csproj")
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
    .AddDotnetProject("customerapi", "../../customer/apis/Customer.Api/Customer.Api.csproj")
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
    .AddDotnetProject("customerprocessors", "../../customer/processors/Customer.Processors/Customer.Processors.csproj")
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
    .AddDotnetProject("customerjobs", "../../customer/jobs/Customer.Jobs/Customer.Jobs.csproj")
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
    .AddDotnetProject("locationinfrastructure", "../../location/shared/Location.Infrastructure/Location.Infrastructure.csproj")
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
    .AddDotnetProject("locationapi", "../../location/apis/Location.Api/Location.Api.csproj")
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
    .AddDotnetProject("locationprocessors", "../../location/processors/Location.Processors/Location.Processors.csproj")
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
    .AddDotnetProject("locationjobs", "../../location/jobs/Location.Jobs/Location.Jobs.csproj")
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
    .AddDotnetProject("marketplaceinfrastructure", "../../marketplace/shared/Marketplace.Infrastructure/Marketplace.Infrastructure.csproj")
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
    .AddDotnetProject("marketplaceapi", "../../marketplace/apis/Marketplace.Api/Marketplace.Api.csproj")
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
    .AddDotnetProject("marketplaceprocessors", "../../marketplace/processors/Marketplace.Processors/Marketplace.Processors.csproj")
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
    .AddDotnetProject("marketplacejobs", "../../marketplace/jobs/Marketplace.Jobs/Marketplace.Jobs.csproj")
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
    .AddDotnetProject("msteamsinfrastructure", "../../msteams/shared/MsTeams.Infrastructure/MsTeams.Infrastructure.csproj")
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
    .AddDotnetProject("msteamsapi", "../../msteams/apis/MsTeams.Api/MsTeams.Api.csproj")
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
    .AddDotnetProject("msteamsprocessors", "../../msteams/processors/MsTeams.Processors/MsTeams.Processors.csproj")
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
    .AddDotnetProject("msteamsjobs", "../../msteams/jobs/MsTeams.Jobs/MsTeams.Jobs.csproj")
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
    .AddDotnetProject("organizationinfrastructure", "../../organization/shared/Organization.Infrastructure/Organization.Infrastructure.csproj")
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
    .AddDotnetProject("organizationapi", "../../organization/apis/Organization.Api/Organization.Api.csproj")
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
    .AddDotnetProject("organizationprocessors", "../../organization/processors/Organization.Processors/Organization.Processors.csproj")
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
    .AddDotnetProject("organizationjobs", "../../organization/jobs/Organization.Jobs/Organization.Jobs.csproj")
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
    .AddDotnetProject("slackinfrastructure", "../../slack/shared/Slack.Infrastructure/Slack.Infrastructure.csproj")
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
    .AddDotnetProject("slackapi", "../../slack/apis/Slack.Api/Slack.Api.csproj")
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
    .AddDotnetProject("slackprocessors", "../../slack/processors/Slack.Processors/Slack.Processors.csproj")
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
    .AddDotnetProject("slackjobs", "../../slack/jobs/Slack.Jobs/Slack.Jobs.csproj")
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
    .AddDotnetProject("teaminfrastructure", "../../team/shared/Team.Infrastructure/Team.Infrastructure.csproj")
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
    .AddDotnetProject("teamapi", "../../team/apis/Team.Api/Team.Api.csproj")
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
    .AddDotnetProject("teamprocessors", "../../team/processors/Team.Processors/Team.Processors.csproj")
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
    .AddDotnetProject("teamjobs", "../../team/jobs/Team.Jobs/Team.Jobs.csproj")
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
    .AddDotnetProject("gateway", "../../gateway/apis/Gateway/Gateway.csproj")
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
    IResourceBuilder<DotnetProjectResource> project,
    IResourceBuilder<DotnetProjectResource> bookingApi,
    IResourceBuilder<DotnetProjectResource> coreApi,
    IResourceBuilder<DotnetProjectResource> customerApi,
    IResourceBuilder<DotnetProjectResource> locationApi,
    IResourceBuilder<DotnetProjectResource> marketplaceApi,
    IResourceBuilder<DotnetProjectResource> msteamsApi,
    IResourceBuilder<DotnetProjectResource> organizationApi,
    IResourceBuilder<DotnetProjectResource> slackApi,
    IResourceBuilder<DotnetProjectResource> teamApi)
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
