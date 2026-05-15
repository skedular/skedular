using Enterprise.Shared.Configurations;
using Enterprise.Shared.HealthCheck;
using Microsoft.Extensions.Hosting;
using Projects;
using DomainAppHostEnvironmentVariables = Enterprise.Shared.DomainAppHostEnvironmentVariables;

await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", ".env"), CancellationToken.None);
await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".env"), CancellationToken.None);

var builder = DistributedApplication.CreateBuilder(args);
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
                 "redpanda:33145", "--advertise-rpc-addr", "redpanda:33145"
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
var useFakeDependencies = DomainAppHostEnvironmentVariables.IsFakeDependenciesEnabled();

var sharedInfrastructure = builder
    .AddProject<Infrastructure_Shared>("infrastructureshared")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WaitFor(redpanda);

var msTeamsFakeDependencies = useFakeDependencies
    ? builder
        .AddProject<MsTeams_Domain_FakeDependencies>("msteamsfakedependencies")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
        .WithHttpHealthCheck(Constants.ReadinessPath)
    : null;

var msTeamsDatabase = postgres.AddDatabase("msteamsdb");
var msTeamsInfrastructure = builder
    .AddProject<Msteams_Infrastructure>("msteamsinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msTeamsDatabase)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(msTeamsDatabase);

var msTeamsApi = builder
    .AddProject<Msteams_Api>("msteamsapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msTeamsDatabase)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(msTeamsDatabase)
    .WaitForCompletion(msTeamsInfrastructure);

var msTeamsProcessors = builder
    .AddProject<Msteams_Processors>("msteamsprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msTeamsDatabase)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(msTeamsDatabase)
    .WaitForCompletion(msTeamsInfrastructure);

var msTeamsJobs = builder
    .AddProject<Msteams_Jobs>("msteamsjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msTeamsDatabase)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(msTeamsDatabase)
    .WaitForCompletion(msTeamsInfrastructure);

if (useFakeDependencies)
{
    ArgumentNullException.ThrowIfNull(msTeamsFakeDependencies);

    msTeamsApi.WaitFor(msTeamsFakeDependencies);
    msTeamsProcessors.WaitFor(msTeamsFakeDependencies);
    msTeamsJobs.WaitFor(msTeamsFakeDependencies);
}

await builder.Build().RunAsync();
