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

var teamFakeDependencies = useFakeDependencies
    ? builder
        .AddProject<Team_Domain_FakeDependencies>("teamfakedependencies")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
        .WithHttpHealthCheck(Constants.ReadinessPath)
    : null;

var teamDatabase = postgres.AddDatabase("teamdb");
var teamInfrastructure = builder
    .AddProject<Team_Infrastructure>("teaminfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(kafka)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(sharedInfrastructure)
    .WaitFor(teamDatabase);

var teamApi = builder
    .AddProject<Team_Api>("teamapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(kafka)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(teamDatabase)
    .WaitForCompletion(teamInfrastructure);

var teamProcessors = builder
    .AddProject<Team_Processors>("teamprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(kafka)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(teamDatabase)
    .WaitForCompletion(teamInfrastructure);

var teamJobs = builder
    .AddProject<Team_Jobs>("teamjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(kafka)
    .WaitFor(temporal)
    .WaitFor(redis)
    .WaitFor(teamDatabase)
    .WaitForCompletion(teamInfrastructure);

if (useFakeDependencies)
{
    ArgumentNullException.ThrowIfNull(teamFakeDependencies);

    teamApi.WaitFor(teamFakeDependencies);
    teamProcessors.WaitFor(teamFakeDependencies);
    teamJobs.WaitFor(teamFakeDependencies);
}

await builder.Build().RunAsync();
