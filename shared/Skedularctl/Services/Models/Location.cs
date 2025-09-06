using CsvHelper.Configuration;

namespace Skedularctl.Services.Models;

public class Location(
    string type,
    string url,
    string title,
    string subtitle,
    string description,
    string contactPerson,
    string contactPhone,
    string address,
    string area,
    string people,
    string emails,
    string websites)
{
    public string Type { get; set; } = type;
    public string Url { get; set; } = url;
    public string Title { get; set; } = title;
    public string Subtitle { get; set; } = subtitle;
    public string Description { get; set; } = description;
    public string ContactPerson { get; set; } = contactPerson;
    public string ContactPhone { get; set; } = contactPhone;
    public string Address { get; set; } = address;
    public string Area { get; set; } = area;
    public string People { get; set; } = people;
    public string Emails { get; set; } = emails;
    public string Websites { get; set; } = websites;
}

public class LocationMap : ClassMap<Location>
{
    public LocationMap()
    {
        Map(m => m.Type).Index(0).Name("Type");
        Map(m => m.Url).Index(1).Name("Url");
        Map(m => m.Title).Index(2).Name("Title");
        Map(m => m.Subtitle).Index(3).Name("Subtitle");
        Map(m => m.Description).Index(4).Name("Description");
        Map(m => m.ContactPerson).Index(5).Name("ContactPerson");
        Map(m => m.ContactPhone).Index(6).Name("ContactPhone");
        Map(m => m.Address).Index(7).Name("Address");
        Map(m => m.Area).Index(8).Name("Area");
        Map(m => m.People).Index(9).Name("People");
        Map(m => m.Emails).Index(10).Name("Emails");
        Map(m => m.Websites).Index(11).Name("Websites");
    }
}
