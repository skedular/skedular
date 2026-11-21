using Aspire.Hosting.Dotnet;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;
using Constants = Enterprise.Shared.HealthCheck.Constants;
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

var bookingFakeDependencies = useFakeDependencies
    ? builder
        .AddDotnetProject("bookingfakedependencies", "../Booking.Domain.FakeDependencies/Booking.Domain.FakeDependencies.csproj")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
        .WithHttpHealthCheck(Constants.ReadinessPath)
    : null;

var bookingDatabase = postgres.AddDatabase("bookingdb");
var bookingInfrastructure = builder
    .AddDotnetProject("bookinginfrastructure", "../../shared/Booking.Infrastructure/Booking.Infrastructure.csproj")
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
    .AddDotnetProject("bookingapi", "../../apis/Booking.Api/Booking.Api.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(sharedInfrastructure)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);

var bookingProcessors = builder
    .AddDotnetProject("bookingprocessors", "../../processors/Booking.Processors/Booking.Processors.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(sharedInfrastructure)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);

var bookingJobs = builder
    .AddDotnetProject("bookingjobs", "../../jobs/Booking.Jobs/Booking.Jobs.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(sharedInfrastructure)
    .WithReference(kafka)
    .WithEnvironment("Kafka__SchemaRegistry__Url", schemaRegistryUrl)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(redpanda)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);

if (useFakeDependencies)
{
    ArgumentNullException.ThrowIfNull(bookingFakeDependencies);

    bookingApi.WaitFor(bookingFakeDependencies);
    bookingProcessors.WaitFor(bookingFakeDependencies);
    bookingJobs.WaitFor(bookingFakeDependencies);

    ConfigureSharedInfrastructureGrpc(bookingApi, bookingFakeDependencies);
    ConfigureSharedInfrastructureGrpc(bookingProcessors, bookingFakeDependencies);
    ConfigureSharedInfrastructureGrpc(bookingJobs, bookingFakeDependencies);
}

await builder.Build().RunAsync();

static void ConfigureSharedInfrastructureGrpc(
    IResourceBuilder<DotnetProjectResource> project,
    IResourceBuilder<DotnetProjectResource> fakeDependencies)
{
    project.WithEnvironment(context =>
    {
        context.EnvironmentVariables["Core__GrpcUrl"] = fakeDependencies.GetEndpoint("Grpc");
        context.EnvironmentVariables["Organization__GrpcUrl"] = fakeDependencies.GetEndpoint("Grpc");
        return Task.CompletedTask;
    });
}
