using System.Reflection;
using Enterprise.Shared.Kafka;

namespace Enterprise.Shared.UnitTests.Kafka.Consume.KafkaNamingTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetClientIdShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Use_Hostname_When_Available(KafkaClientNaming sut, string clientId)
    {
        Environment.SetEnvironmentVariable("HOSTNAME", clientId);
        sut.GetClientId().ShouldBe(clientId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_Application_Name_When_Hostname_Not_Available(KafkaClientNaming sut)
    {
        var expected = Assembly.GetEntryAssembly()!.GetName().Name;
        expected.ShouldNotBeEmpty("Application must have a name for this test");

        Environment.SetEnvironmentVariable("HOSTNAME", null);

        sut.GetClientId().ShouldBe(expected);
    }
}
