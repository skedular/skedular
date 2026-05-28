using AutoFixture;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Testing.Shared.Fixtures;

internal sealed class FakeHostEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = Environments.Production;
    public string ApplicationName { get; set; } = "Enterprise.Shared.UnitTests";
    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public IFileProvider ContentRootFileProvider { get; set; } = null!;
}

public class FakeHostEnvironmentFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) => fixture.Register<IHostEnvironment>(() => new FakeHostEnvironment());
}
