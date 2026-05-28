using CsvHelper.Configuration;

namespace WebScrapper.Models;

public class Location
{
    public Location()
    {
    }

    public Location(
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
        Type = type;
        Url = url;
        Title = title;
        Subtitle = subtitle;
        Description = description;
        ContactPerson = contactPerson;
        ContactPhone = contactPhone;
        Address = address;
        Area = area;
        People = people;
        Emails = emails;
        Websites = websites;
    }

    public string Type { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string People { get; set; } = string.Empty;
    public string Emails { get; set; } = string.Empty;
    public string Websites { get; set; } = string.Empty;
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
