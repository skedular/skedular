using System.Reflection;
using Enterprise.Shared.Kafka;
using FluentAssertions;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Kafka.Consume.KafkaNamingTests;

public class GetClientIdShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Use_Hostname_When_Available(KafkaClientNaming sut, string clientId)
    {
        Environment.SetEnvironmentVariable("HOSTNAME", clientId);
        sut.GetClientId().Should().Be(clientId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_Application_Name_When_Hostname_Not_Available(KafkaClientNaming sut)
    {
        var expected = Assembly.GetEntryAssembly()!.GetName().Name;
        expected.Should().NotBeEmpty("Application must have a name for this test");

        Environment.SetEnvironmentVariable("HOSTNAME", null);

        sut.GetClientId().Should().Be(expected);
    }
}
