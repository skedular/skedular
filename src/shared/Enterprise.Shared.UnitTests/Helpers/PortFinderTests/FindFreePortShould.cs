using Enterprise.Shared.Helpers;

namespace Enterprise.Shared.UnitTests.Helpers.PortFinderTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FindFreePortShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_a_valid_port_number(PortFinder sut)
    {
        var port = sut.FindFreePort();

        port.ShouldBeGreaterThan(0);
        port.ShouldBeLessThanOrEqualTo(65535);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_different_port_on_each_call(PortFinder sut)
    {
        var ports = Enumerable.Range(0, 5).Select(_ => sut.FindFreePort()).ToList();

        // Very unlikely that all 5 ports are the same
        ports.Distinct().Count().ShouldBeGreaterThan(1);
    }
}
