using CommandLine;
using Microsoft.Extensions.Hosting;

namespace GraphQLSchemaGenerator.GraphQL;

[Verb("graphql-schema-generate", HelpText = "Generate GraphQL schema")]
public class SchemaGenerateOptions
{
    [Option("output", Required = true, HelpText = "The output schema path")]
    public string OutputPath { get; set; } = string.Empty;
}

public class SchemaGenerateHandler(SchemaGenerateOptions options)
{
    public async Task HandleAsync()
    {
        var rootPath = string.IsNullOrWhiteSpace(options.OutputPath)
            ? Path.Join(
                Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "gateway", "apis", "Gateway", "schemas")
            : options.OutputPath;
        string[] schemaExportBaseArgs = ["schema", "export", "--output"];

        await Task.WhenAll(
            Billing.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Billing.graphql")]).ToArray()),
            Booking.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Booking.graphql")]).ToArray()),
            Customer.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Customer.graphql")]).ToArray()),
            Location.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Location.graphql")]).ToArray()),
            MsTeams.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "MsTeams.graphql")]).ToArray()),
            Organization.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Organization.graphql")]).ToArray()),
            Notification.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Notification.graphql")]).ToArray()),
            Payment.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Payment.graphql")]).ToArray()),
            Slack.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Slack.graphql")]).ToArray()),
            Team.Api.Program.CreateHostBuilder([]).Build().RunWithGraphQLCommandsAsync(
                schemaExportBaseArgs.Concat([Path.Join(rootPath, "Team.graphql")]).ToArray())
        );
    }
}
