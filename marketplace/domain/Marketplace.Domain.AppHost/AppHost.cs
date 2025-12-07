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
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka);

var marketplaceDatabase = postgres.AddDatabase("marketplacedb");
var marketplaceInfrastructure = builder
    .AddProject<Marketplace_Infrastructure>("marketplaceinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(marketplaceDatabase);

builder
    .AddProject<Marketplace_Api>("marketplaceapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure);

builder
    .AddProject<Marketplace_Processors>("marketplaceprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure);

builder
    .AddProject<Marketplace_Jobs>("marketplacejobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure);

await builder.Build().RunAsync();
