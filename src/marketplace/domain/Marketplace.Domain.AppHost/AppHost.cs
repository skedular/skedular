using Enterprise.Shared.Configurations;
using Enterprise.Shared.HealthCheck;
using Microsoft.Extensions.Hosting;
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
var useFakeDependencies = DomainAppHostEnvironmentVariables.IsFakeDependenciesEnabled();

var sharedInfrastructure = builder
    .AddDotnetProject("infrastructureshared", "../../../shared/Infrastructure.Shared/Infrastructure.Shared.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WaitFor(redpanda);

var marketplaceFakeDependencies = useFakeDependencies
    ? builder
        .AddDotnetProject("marketplacefakedependencies", "../Marketplace.Domain.FakeDependencies/Marketplace.Domain.FakeDependencies.csproj")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
        .WithHttpHealthCheck(Constants.ReadinessPath)
    : null;

var marketplaceDatabase = postgres.AddDatabase("marketplacedb");
var marketplaceInfrastructure = builder
    .AddDotnetProject("marketplaceinfrastructure", "../../shared/Marketplace.Infrastructure/Marketplace.Infrastructure.csproj")
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
    .AddDotnetProject("marketplaceapi", "../../apis/Marketplace.Api/Marketplace.Api.csproj")
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

var marketplaceProcessors = builder
    .AddDotnetProject("marketplaceprocessors", "../../processors/Marketplace.Processors/Marketplace.Processors.csproj")
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
    .AddDotnetProject("marketplacejobs", "../../jobs/Marketplace.Jobs/Marketplace.Jobs.csproj")
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

if (useFakeDependencies)
{
    ArgumentNullException.ThrowIfNull(marketplaceFakeDependencies);

    marketplaceApi.WaitFor(marketplaceFakeDependencies);
    marketplaceProcessors.WaitFor(marketplaceFakeDependencies);
    marketplaceJobs.WaitFor(marketplaceFakeDependencies);
}

await builder.Build().RunAsync();
