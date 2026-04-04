using Enterprise.Shared.Configurations;
using Enterprise.Shared.HealthCheck;
using Microsoft.Extensions.Hosting;
using Projects;
using DomainAppHostEnvironmentVariables = Enterprise.Shared.DomainAppHostEnvironmentVariables;

await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", ".env"), CancellationToken.None);
await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".env"), CancellationToken.None);

var builder = DistributedApplication.CreateBuilder(args);
var kafka = builder.AddKafka("kafka").WithKafkaUI();
var postgres = builder.AddPostgres("postgres").WithImage("postgis/postgis", "16-3.4").WithPgAdmin();
var temporal = builder.AddTemporalServerContainer("temporal");
#pragma warning disable ASPIRECERTIFICATES001
var redis = builder.AddRedis("redis").WithoutHttpsCertificate();
#pragma warning restore ASPIRECERTIFICATES001
var useFakeDependencies = DomainAppHostEnvironmentVariables.IsFakeDependenciesEnabled();

var sharedInfrastructure = builder
    .AddProject<Infrastructure_Shared>("infrastructureshared")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WaitFor(kafka);

var locationFakeDependencies = useFakeDependencies
    ? builder
        .AddProject<Location_Domain_FakeDependencies>("locationfakedependencies")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
        .WithHttpHealthCheck(Constants.ReadinessPath)
    : null;

var locationDatabase = postgres.AddDatabase("locationdb");
var locationInfrastructure = builder
    .AddProject<Location_Infrastructure>("locationinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(kafka)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(locationDatabase);

var locationApi = builder
    .AddProject<Location_Api>("locationapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(kafka)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(locationDatabase)
    .WaitForCompletion(locationInfrastructure);

var locationProcessors = builder
    .AddProject<Location_Processors>("locationprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(kafka)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(locationDatabase)
    .WaitForCompletion(locationInfrastructure);

var locationJobs = builder
    .AddProject<Location_Jobs>("locationjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(kafka)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(locationDatabase)
    .WaitForCompletion(locationInfrastructure);

if (useFakeDependencies)
{
    ArgumentNullException.ThrowIfNull(locationFakeDependencies);

    locationApi.WaitFor(locationFakeDependencies);
    locationProcessors.WaitFor(locationFakeDependencies);
    locationJobs.WaitFor(locationFakeDependencies);
}

await builder.Build().RunAsync();
