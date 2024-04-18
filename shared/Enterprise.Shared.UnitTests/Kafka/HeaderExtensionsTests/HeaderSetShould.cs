using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using FluentAssertions;
using Xunit;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

public class SetShould
{
    [Fact]
    public void Add_Header_If_Not_Present()
    {
        var headers = new Headers { new Header("one", []), new Header("two", []), new Header("three", []) };

        headers.Set("my header", "my value");

        var header = headers[3];
        header.Key.Should().Be("my header");
        var valueBytes = header.GetValueBytes();
        Encoding.UTF8.GetString(valueBytes).Should().Be("my value");
    }

    [Fact]
    public void Replace_Header_If_Present()
    {
        var headers = new Headers { new Header("one", []), new Header("my header", []), new Header("three", []) };

        headers.Set("my header", "my value");

        var header = headers[2];
        header.Key.Should().Be("my header");
        var valueBytes = header.GetValueBytes();
        Encoding.UTF8.GetString(valueBytes).Should().Be("my value");
    }
}
