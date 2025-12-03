using Enterprise.Shared.Configurations;
using Enterprise.Shared.HealthCheck;
using Projects;

await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", ".env"), CancellationToken.None);
await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".env"), CancellationToken.None);

var builder = DistributedApplication.CreateBuilder(args);
var kafka = builder.AddKafka("kafka").WithKafkaUI();
var postgres = builder.AddPostgres("postgres").WithImage("postgis/postgis", "16-3.4").WithPgAdmin();
var temporal = builder.AddTemporalServerContainer("temporal");
var redis = builder.AddRedis("redis");

var customerDatabase = postgres.AddDatabase("customerdb");
var sharedInfrastructure = builder
    .AddProject<Infrastructure_Shared>("infrastructureshared")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka);

var customerInfrastructure = builder
    .AddProject<Customer_Infrastructure>("customerinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(customerDatabase);

builder
    .AddProject<Customer_Api>("customerapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitForCompletion(customerInfrastructure);

builder
    .AddProject<Customer_Jobs>("customerprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitForCompletion(customerInfrastructure);

builder
    .AddProject<Customer_Jobs>("customerjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitForCompletion(customerInfrastructure);

await builder.Build().RunAsync();
