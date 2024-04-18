using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using FluentAssertions;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.HeaderPropagatorFunctionsTests;

public class InjectShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_New_Values(HeaderPropagatorFunctions functions)
    {
        var destination = new Headers();
        functions.Inject(destination, "my key", "my value");
        destination[0].Key.Should().Be("my key");
        var valueBytes = destination[0].GetValueBytes();
        Encoding.UTF8.GetString(valueBytes).Should().Be("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Overwrite_Previous_Keys(HeaderPropagatorFunctions functions)
    {
        // we need to overwrite keys where the message has been previously sent, such as retries
        var destination = new Headers { { "my key", "my first value"u8.ToArray() } };
        functions.Inject(destination, "my key", "my second value");

        destination[0].Key.Should().Be("my key");
        var valueBytes = destination[0].GetValueBytes();
        Encoding.UTF8.GetString(valueBytes).Should().Be("my second value");
    }
}
