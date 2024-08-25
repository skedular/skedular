using Enterprise.Shared.Application.WebHostService;

namespace MsTeams.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program : WebHostServiceBase<Program>
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).Build().RunAsync();

    // ReSharper disable once MemberCanBePrivate.Global
    public static IHostBuilder CreateHostBuilder(string[] args) => CreateHostBuilder<Startup>(args);
}
