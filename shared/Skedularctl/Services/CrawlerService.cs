using CommandLine;

namespace Skedularctl.Services;

[Verb("crawl")]
public class CrawlOptions;

public interface ICrawlerService
{
    Task HandleAsync(CrawlOptions options, CancellationToken cancellationToken);
}

public class CrawlerService : ICrawlerService
{
    public Task HandleAsync(CrawlOptions options, CancellationToken cancellationToken) => throw new NotImplementedException();
}
