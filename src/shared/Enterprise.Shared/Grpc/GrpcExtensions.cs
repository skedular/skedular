using Grpc.Core;

namespace Enterprise.Shared.Grpc;

public static class GrpcExtensions
{
    extension(string apiKey)
    {
        public Metadata CreateMetadata()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
            return new Metadata
            {
                { Constants.ApiKey, apiKey },
            };
        }

        public Metadata CreateMetadata(string verifiableToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
            return apiKey.CreateMetadata().AddVerifiableToken(verifiableToken);
        }
    }

    extension(Metadata metadata)
    {
        public Metadata AddVerifiableToken(string verifiableToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);
            metadata.Add(Constants.VerifiableTokenKey, verifiableToken);

            return metadata;
        }
    }
}
