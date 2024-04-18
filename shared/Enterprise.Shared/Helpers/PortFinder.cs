using System.Net;
using System.Net.Sockets;

namespace Enterprise.Shared.Helpers;

public static class PortFinder
{
    public static int FindFreePort()
    {
        var tcpListener = new TcpListener(IPAddress.Loopback, 0);

        tcpListener.Start();
        var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        tcpListener.Stop();

        return port;
    }
}
