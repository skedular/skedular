using CommandLine;
using Unityhubctl.Events.Generator;

namespace Unityhubctl;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var parserResult = Parser.Default
            .ParseArguments<ProtobufEventMetadataGenerateOptions>(args);

        await Task.WhenAll(new Task[]
        {
            parserResult.WithParsedAsync(async options =>
                await new ProtobufEventMetadataGenerateHandler(options).HandleAsync())
        });
    }
}
