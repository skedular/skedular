using Grpc.Core;

namespace Enterprise.Shared.Grpc;

public static class GrpcExtensions
{
    public static Metadata CreateMetadata(this string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        return new Metadata { { Constants.ApiKey, apiKey } };
    }

    public static Metadata CreateMetadata(this string apiKey, string verifiableToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        return CreateMetadata(apiKey).AddVerifiableToken(verifiableToken);
    }

    public static Metadata AddVerifiableToken(this Metadata metadata, string verifiableToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);
        metadata.Add(Constants.VerifiableTokenKey, verifiableToken);

        return metadata;
    }
}
