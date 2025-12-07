using Enterprise.Shared.Configurations;
using Enterprise.Shared.HealthCheck;
using Microsoft.Extensions.Hosting;
using Projects;

await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", ".env"), CancellationToken.None);
await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".env"), CancellationToken.None);

var builder = DistributedApplication.CreateBuilder(args);
var kafka = builder.AddKafka("kafka").WithKafkaUI();
var postgres = builder.AddPostgres("postgres").WithImage("postgis/postgis", "16-3.4").WithPgAdmin();
var temporal = builder.AddTemporalServerContainer("temporal");
var redis = builder.AddRedis("redis");

var sharedInfrastructure = builder
    .AddProject<Infrastructure_Shared>("infrastructureshared")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka);

var organizationDatabase = postgres.AddDatabase("organizationdb");
var organizationInfrastructure = builder
    .AddProject<Organization_Infrastructure>("organizationinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(organizationDatabase);

builder
    .AddProject<Organization_Api>("organizationapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure);

builder
    .AddProject<Organization_Processors>("organizationprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure);

builder
    .AddProject<Organization_Jobs>("organizationjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure);

await builder.Build().RunAsync();
