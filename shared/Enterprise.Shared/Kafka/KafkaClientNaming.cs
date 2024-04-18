using System.Reflection;

namespace Enterprise.Shared.Kafka;

public interface IKafkaClientNaming
{
    string GetClientId();
}

public class KafkaClientNaming : IKafkaClientNaming
{
    public string GetClientId()
    {
        var clientId = Environment.GetEnvironmentVariable("HOSTNAME");

        if (!string.IsNullOrEmpty(clientId))
        {
            return clientId;
        }

        clientId = Assembly.GetEntryAssembly()!.GetName().Name;

        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

        return clientId;
    }
}
