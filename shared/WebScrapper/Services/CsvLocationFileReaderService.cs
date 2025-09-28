using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using WebScrapper.Models;

namespace WebScrapper.Services;

public interface ICsvLocationFileReaderService
{
    ICollection<Location> ReadLocations();
}

public class CsvLocationFileReaderService : ICsvLocationFileReaderService
{
    public ICollection<Location> ReadLocations()
    {
        using var reader = new StreamReader("/Users/morteza/Downloads/locations-output.csv");
        using var csv = new CsvReader(
            reader,
            new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "|", ShouldQuote = _ => true, NewLine = Environment.NewLine });
        csv.Context.RegisterClassMap<LocationMap>();

        return csv.GetRecords<Location>().ToList();
    }
}
