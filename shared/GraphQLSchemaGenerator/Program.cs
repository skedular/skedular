using CommandLine;
using GraphQLSchemaGenerator.GraphQL;

namespace GraphQLSchemaGenerator;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var parserResult = Parser.Default
            .ParseArguments<SchemaGenerateOptions>(args);

        await Task.WhenAll(new Task[]
        {
            parserResult.WithParsedAsync(async options => await new SchemaGenerateHandler(options).HandleAsync())
        });
    }
}
