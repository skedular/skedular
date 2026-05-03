namespace Enterprise.Shared.GraphQL.Handlers;

public sealed class RewriteHostHandler(Uri target) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null)
        {
            request.RequestUri = new UriBuilder(target) { Path = request.RequestUri.AbsolutePath, Query = request.RequestUri.Query.TrimStart('?') }
                .Uri;
        }

        return base.SendAsync(request, cancellationToken);
    }
}
