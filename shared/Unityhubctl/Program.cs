using CommandLine;
using Unityhubctl.Events.Generator;
using Unityhubctl.GraphQL.Generator;

namespace Unityhubctl;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var parserResult = Parser.Default
            .ParseArguments<ProtobufEventMetadataGenerateOptions, GraphQLServicesGenerateOptions>(args);

        await Task.WhenAll(new Task[]
        {
            parserResult.WithParsedAsync<ProtobufEventMetadataGenerateOptions>(async options =>
                await new ProtobufEventMetadataGenerateHandler(options).HandleAsync()),
            parserResult.WithParsedAsync<GraphQLServicesGenerateOptions>(async options =>
                await new GraphQLServicesGenerateHandler(options).HandleAsync())
        });
    }
}
