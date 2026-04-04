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

var msTeamsFakeDependencies = useSharedInfrastructureGrpc
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
    .WaitForCompletion(msTeamsInfrastructure);

var msTeamsProcessors = builder
    .AddProject<Msteams_Processors>("msteamsprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msTeamsDatabase)
    .WaitForCompletion(msTeamsInfrastructure);

var msTeamsJobs = builder
    .AddProject<Msteams_Jobs>("msteamsjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Development)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msTeamsDatabase)
    .WaitForCompletion(msTeamsInfrastructure);

if (useSharedInfrastructureGrpc)
{
    ArgumentNullException.ThrowIfNull(msTeamsFakeDependencies);

    msTeamsApi.WaitFor(msTeamsFakeDependencies);
    msTeamsProcessors.WaitFor(msTeamsFakeDependencies);
    msTeamsJobs.WaitFor(msTeamsFakeDependencies);
}

await builder.Build().RunAsync();
