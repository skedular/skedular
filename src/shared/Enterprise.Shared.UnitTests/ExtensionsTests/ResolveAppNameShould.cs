namespace Enterprise.Shared.UnitTests.ExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveAppNameShould
{
    [Theory]
    [InlineData("AllApis")]
    [InlineData("AllJobs")]
    [InlineData("AllProcessors")]
    public void Use_The_All_In_One_Process_Assembly(string entryAssembly) =>
        Extensions.ResolveAppName(entryAssembly, "Booking.Api", "fallback").ShouldBe(entryAssembly);

    [Fact]
    public void Use_The_Host_Assembly_For_A_Standalone_Process() =>
        Extensions.ResolveAppName("Booking.Api", "Booking.Api", "fallback").ShouldBe("Booking.Api");
}
