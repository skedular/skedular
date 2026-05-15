namespace Testing.Shared.IntegrationTests;

public static class HttpClientExtensions
{
    public static async Task WaitForSuccessfulGetAsync(
        this HttpClient httpClient,
        string requestUri,
        CancellationToken cancellationToken,
        TimeSpan? timeout = null,
        TimeSpan? pollInterval = null)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        pollInterval ??= TimeSpan.FromMilliseconds(250);

        var deadline = TimeProvider.System.GetUtcNow().Add(timeout.Value);
        Exception? lastException = null;

        while (TimeProvider.System.GetUtcNow() < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using var response = await httpClient.GetAsync(requestUri, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            await Task.Delay(pollInterval.Value, cancellationToken);
        }

        throw new TimeoutException(
            $"GET '{requestUri}' did not succeed at '{httpClient.BaseAddress}' within {timeout.Value}.",
            lastException);
    }
}
