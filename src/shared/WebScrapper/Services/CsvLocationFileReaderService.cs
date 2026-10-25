using CsvHelper;
using WebScrapper.Models;

namespace WebScrapper.Services;

public interface ICsvLocationFileReaderService
{
    IReadOnlyList<Location> ReadLocations();
}

public class CsvLocationFileReaderService : ICsvLocationFileReaderService
{
    public IReadOnlyList<Location> ReadLocations()
    {
        using var reader = new StreamReader("/Users/morteza/Downloads/locations-output.csv");
        using var csv = CsvReader.Create(
            reader,
            new CsvReaderOptions
            {
                Separator = '|',
            });

        return [.. csv.GetRecords<Location>()];
    }
}
