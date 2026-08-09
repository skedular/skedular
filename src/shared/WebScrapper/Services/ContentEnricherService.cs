using System.Text.RegularExpressions;

namespace WebScrapper.Services;

public interface IContentEnricherService
{
    IReadOnlyList<string> ExtractEmails(string content);
    IReadOnlyList<string> ExtractWebsites(string content);
}

public class ContentEnricherService : IContentEnricherService
{
    public IReadOnlyList<string> ExtractEmails(string content)
    {
        var emailRegex = new Regex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", RegexOptions.IgnoreCase);


        var matched = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        AddMatches(emailRegex);

        return [.. matched];

        void AddMatches(Regex regex)
        {
            foreach (Match m in regex.Matches(content))
            {
                if (m.Success)
                {
                    matched.Add(m.Value.Trim());
                }
            }
        }
    }

    public IReadOnlyList<string> ExtractWebsites(string content)
    {
        var urlRegex = new Regex(@"\b((https?|ftp):\/\/[^\s/$.?#].[^\s]*)\b", RegexOptions.IgnoreCase);
        var wwwRegex = new Regex(@"\b(www\.[^\s/$.?#].[^\s]*)\b", RegexOptions.IgnoreCase);
        var bareDomainRegex = new Regex(@"\b((?:[a-z0-9-]+\.)+[a-z]{2,})(?:/[^\s]*)?\b", RegexOptions.IgnoreCase);

        var matched = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        AddMatches(urlRegex);
        AddMatches(wwwRegex);
        AddMatches(bareDomainRegex);

        return [.. matched];

        void AddMatches(Regex regex)
        {
            foreach (Match m in regex.Matches(content))
            {
                if (m.Success)
                {
                    matched.Add(m.Value.Trim());
                }
            }
        }
    }
}
