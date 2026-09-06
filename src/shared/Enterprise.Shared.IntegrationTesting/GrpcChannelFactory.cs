using Grpc.Net.Client;

namespace Enterprise.Shared.IntegrationTesting;

public static class GrpcChannelFactory
{
    public static GrpcChannel Create(string address) =>
        GrpcChannel.ForAddress(
            address,
            new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                },
            });
}
