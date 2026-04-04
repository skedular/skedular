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
var useSharedInfrastructureGrpc = DomainAppHostEnvironmentVariables.IsSharedInfrastructureGrpcEnabled();

var sharedInfrastructure = builder
    .AddProject<Infrastructure_Shared>("infrastructureshared")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka);

var slackFakeDependencies = useSharedInfrastructureGrpc
    ? builder
        .AddProject<Slack_Domain_FakeDependencies>("slackfakedependencies")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
        .WithHttpHealthCheck(Constants.ReadinessPath)
    : null;

var slackDatabase = postgres.AddDatabase("slackdb");
var slackInfrastructure = builder
    .AddProject<Slack_Infrastructure>("slackinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(slackDatabase);

var slackApi = builder
    .AddProject<Slack_Api>("slackapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitForCompletion(slackInfrastructure);

var slackProcessors = builder
    .AddProject<Slack_Processors>("slackprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitForCompletion(slackInfrastructure);

var slackJobs = builder
    .AddProject<Slack_Jobs>("slackjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitForCompletion(slackInfrastructure);

if (useSharedInfrastructureGrpc)
{
    ArgumentNullException.ThrowIfNull(slackFakeDependencies);

    slackApi.WaitFor(slackFakeDependencies);
    slackProcessors.WaitFor(slackFakeDependencies);
    slackJobs.WaitFor(slackFakeDependencies);
}

await builder.Build().RunAsync();
