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

/******************************************************************************************************************************/
var bookingDatabase = postgres.AddDatabase("bookingdb");
var bookingInfrastructure = builder
    .AddProject<Booking_Infrastructure>("bookinginfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(bookingDatabase);

var bookingApi = builder
    .AddProject<Booking_Api>("bookingapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);

builder
    .AddProject<Booking_Processors>("bookingprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);

builder
    .AddProject<Booking_Jobs>("bookingjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(bookingDatabase)
    .WaitForCompletion(bookingInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var coreDatabase = postgres.AddDatabase("coredb");
var coreInfrastructure = builder
    .AddProject<Core_Infrastructure>("coreinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(coreDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(coreDatabase);

var coreApi = builder
    .AddProject<Core_Api>("coreapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(coreDatabase)
    .WaitForCompletion(coreInfrastructure);

builder
    .AddProject<Core_Processors>("coreprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(coreDatabase)
    .WaitForCompletion(coreInfrastructure);

builder
    .AddProject<Core_Jobs>("corejobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(coreDatabase)
    .WaitForCompletion(coreInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var customerDatabase = postgres.AddDatabase("customerdb");
var customerInfrastructure = builder
    .AddProject<Customer_Infrastructure>("customerinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(customerDatabase);

var customerApi = builder
    .AddProject<Customer_Api>("customerapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitForCompletion(customerInfrastructure);

builder
    .AddProject<Customer_Processors>("customerprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitForCompletion(customerInfrastructure);

builder
    .AddProject<Customer_Jobs>("customerjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(customerDatabase)
    .WaitForCompletion(customerInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var locationDatabase = postgres.AddDatabase("locationdb");
var locationInfrastructure = builder
    .AddProject<Location_Infrastructure>("locationinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(locationDatabase);

var locationApi = builder
    .AddProject<Location_Api>("locationapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitForCompletion(locationInfrastructure);

builder
    .AddProject<Location_Processors>("locationprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitForCompletion(locationInfrastructure);

builder
    .AddProject<Location_Jobs>("locationjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(locationDatabase)
    .WaitForCompletion(locationInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var marketplaceDatabase = postgres.AddDatabase("marketplacedb");
var marketplaceInfrastructure = builder
    .AddProject<Marketplace_Infrastructure>("marketplaceinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(marketplaceDatabase);

var marketplaceApi = builder
    .AddProject<Marketplace_Api>("marketplaceapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure);

builder
    .AddProject<Marketplace_Processors>("marketplaceprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure);

builder
    .AddProject<Marketplace_Jobs>("marketplacejobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(marketplaceDatabase)
    .WaitForCompletion(marketplaceInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var msteamsDatabase = postgres.AddDatabase("msteamsdb");
var msteamsInfrastructure = builder
    .AddProject<MsTeams_Infrastructure>("msteamsinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msteamsDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(msteamsDatabase);

var msteamsApi = builder
    .AddProject<MsTeams_Api>("msteamsapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msteamsDatabase)
    .WaitForCompletion(msteamsInfrastructure);

builder
    .AddProject<MsTeams_Processors>("msteamsprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msteamsDatabase)
    .WaitForCompletion(msteamsInfrastructure);

builder
    .AddProject<MsTeams_Jobs>("msteamsjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(msteamsDatabase)
    .WaitForCompletion(msteamsInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
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

var organizationApi = builder
    .AddProject<Organization_Api>("organizationapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure);

builder
    .AddProject<Organization_Processors>("organizationprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure);

builder
    .AddProject<Organization_Jobs>("organizationjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(organizationDatabase)
    .WaitForCompletion(organizationInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var slackDatabase = postgres.AddDatabase("slackdb");
var slackInfrastructure = builder
    .AddProject<Slack_Infrastructure>("slackinfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(slackDatabase);

var slackApi = builder
    .AddProject<Slack_Api>("slackapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitForCompletion(slackInfrastructure);

builder
    .AddProject<Slack_Processors>("slackprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitForCompletion(slackInfrastructure);

builder
    .AddProject<Slack_Jobs>("slackjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(slackDatabase)
    .WaitForCompletion(slackInfrastructure);
/******************************************************************************************************************************/

/******************************************************************************************************************************/
var teamDatabase = postgres.AddDatabase("teamdb");
var teamInfrastructure = builder
    .AddProject<Team_Infrastructure>("teaminfrastructure")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitFor(sharedInfrastructure)
    .WaitFor(teamDatabase);

var teamApi = builder
    .AddProject<Team_Api>("teamapi")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitForCompletion(teamInfrastructure);

builder
    .AddProject<Team_Processors>("teamprocessors")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitForCompletion(teamInfrastructure);

builder
    .AddProject<Team_Jobs>("teamjobs")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Booking__GrpcUrl", "http://bookingapi")
    .WithEnvironment("Core__GrpcUrl", "http://coreapi")
    .WithEnvironment("Customer__GrpcUrl", "http://customerapi")
    .WithEnvironment("Location__GrpcUrl", "http://locationapi")
    .WithEnvironment("Marketplace__GrpcUrl", "http://marketplaceapi")
    .WithEnvironment("MsTeams__GrpcUrl", "http://msteamsapi")
    .WithEnvironment("Organization__GrpcUrl", "http://organizationapi")
    .WithEnvironment("Slack__GrpcUrl", "http://slackapi")
    .WithEnvironment("Team__GrpcUrl", "http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(kafka)
    .WithReference(temporal)
    .WithReference(redis)
    .WithReference(teamDatabase)
    .WaitForCompletion(teamInfrastructure);
/******************************************************************************************************************************/

_ = builder
    .AddProject<Gateway>("gateway")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", Environments.Production)
    .WithEnvironment("Subgraphs__Booking__Uri", "https+http://bookingapi/v1/graphql")
    .WithEnvironment("Subgraphs__Core__Uri", "https+http://coreapi/v1/graphql")
    .WithEnvironment("Subgraphs__Customer__Uri", "https+http://customerapi/v1/graphql")
    .WithEnvironment("Subgraphs__Location__Uri", "https+http://locationapi/v1/graphql")
    .WithEnvironment("Subgraphs__Marketplace__Uri", "https+http://marketplaceapi/v1/graphql")
    .WithEnvironment("Subgraphs__MsTeams__Uri", "https+http://msteamsapi/v1/graphql")
    .WithEnvironment("Subgraphs__Organization__Uri", "https+http://organizationapi/v1/graphql")
    .WithEnvironment("Subgraphs__Slack__Uri", "https+http://slackapi/v1/graphql")
    .WithEnvironment("Subgraphs__Team__Uri", "https+http://teamapi/v1/graphql")
    .WithEnvironment("ReverseProxy__Clusters__booking__Destinations__destination1__Address", "https+http://bookingapi")
    .WithEnvironment("ReverseProxy__Clusters__core__Destinations__destination1__Address", "https+http://coreapi")
    .WithEnvironment("ReverseProxy__Clusters__customer__Destinations__destination1__Address", "https+http://customerapi")
    .WithEnvironment("ReverseProxy__Clusters__location__Destinations__destination1__Address", "https+http://locationapi")
    .WithEnvironment("ReverseProxy__Clusters__marketplace__Destinations__destination1__Address", "https+http://marketplaceapi")
    .WithEnvironment("ReverseProxy__Clusters__msteams__Destinations__destination1__Address", "https+http://msteamsapi")
    .WithEnvironment("ReverseProxy__Clusters__organization__Destinations__destination1__Address", "https+http://organizationapi")
    .WithEnvironment("ReverseProxy__Clusters__slack__Destinations__destination1__Address", "https+http://slackapi")
    .WithEnvironment("ReverseProxy__Clusters__team__Destinations__destination1__Address", "https+http://teamapi")
    .WithHttpHealthCheck(Constants.ReadinessPath)
    .WithReference(bookingApi)
    .WaitFor(bookingApi)
    .WithReference(coreApi)
    .WaitFor(coreApi)
    .WithReference(customerApi)
    .WaitFor(customerApi)
    .WithReference(locationApi)
    .WaitFor(locationApi)
    .WithReference(marketplaceApi)
    .WaitFor(marketplaceApi)
    .WithReference(msteamsApi)
    .WaitFor(msteamsApi)
    .WithReference(organizationApi)
    .WaitFor(organizationApi)
    .WithReference(slackApi)
    .WaitFor(slackApi)
    .WithReference(teamApi)
    .WaitFor(teamApi);

await builder.Build().RunAsync();
