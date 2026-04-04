using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;
using Projects;
using Constants = Enterprise.Shared.HealthCheck.Constants;
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
    .WithReference(kafka);

var bookingFakeDependencies = useFakeDependencies
    ? builder
        .AddProject<Booking_Domain_FakeDependencies>("bookingfakedependencies")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
        .WithHttpHealthCheck(Constants.ReadinessPath)
    : null;

var bookingDatabase = postgres.AddDatabase("bookingdb");
var bookingInfrastructure = builder
    .AddProject<Booking_Infrastructure>("bookinginfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(bookingDatabase);

var bookingApi = builder
    .AddProject<Booking_Api>("bookingapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(sharedInfrastructure)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);

var bookingProcessors = builder
    .AddProject<Booking_Processors>("bookingprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(sharedInfrastructure)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);

var bookingJobs = builder
    .AddProject<Booking_Jobs>("bookingjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(sharedInfrastructure)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
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
    IResourceBuilder<ProjectResource> project,
    IResourceBuilder<ProjectResource> fakeDependencies)
{
    project.WithEnvironment(context =>
    {
        context.EnvironmentVariables["Core__GrpcUrl"] = fakeDependencies.GetEndpoint("Grpc");
        context.EnvironmentVariables["Organization__GrpcUrl"] = fakeDependencies.GetEndpoint("Grpc");
        return Task.CompletedTask;
    });
}
