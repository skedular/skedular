using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Helpers;

public interface IPortFinder
{
    int FindFreePort();
}

public class PortFinder(ILogger<PortFinder> logger) : IPortFinder
{
    public int FindFreePort()
    {
        logger.LogDebug("Finding free TCP port on loopback interface");
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);

        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();

        logger.LogInformation("Found free TCP port successfully. Port={Port}", port);
        return port;
    }
}
