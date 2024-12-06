using Enterprise.Shared.Application.WebHostService;

namespace Booking.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program : WebHostServiceBase<Program>
{
    public static async Task Main(string[] args) =>
        await CreateHostBuilder(args).Build().RunWithGraphQLCommandsAsync(args);

    // ReSharper disable once MemberCanBePrivate.Global
    public static IHostBuilder CreateHostBuilder(string[] args) => CreateHostBuilder<Startup>(args);
}
