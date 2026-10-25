using CsvHelper;

namespace WebScrapper.Models;

[CsvRecord]
public partial class Location
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

    [CsvColumn("Type", Index = 0)]
    public string Type { get; set; } = string.Empty;

    [CsvColumn("Url", Index = 1)]
    public string Url { get; set; } = string.Empty;

    [CsvColumn("Title", Index = 2)]
    public string Title { get; set; } = string.Empty;

    [CsvColumn("Subtitle", Index = 3)]
    public string Subtitle { get; set; } = string.Empty;

    [CsvColumn("Description", Index = 4)]
    public string Description { get; set; } = string.Empty;

    [CsvColumn("ContactPerson", Index = 5)]
    public string ContactPerson { get; set; } = string.Empty;

    [CsvColumn("ContactPhone", Index = 6)]
    public string ContactPhone { get; set; } = string.Empty;

    [CsvColumn("Address", Index = 7)]
    public string Address { get; set; } = string.Empty;

    [CsvColumn("Area", Index = 8)]
    public string Area { get; set; } = string.Empty;

    [CsvColumn("People", Index = 9)]
    public string People { get; set; } = string.Empty;

    [CsvColumn("Emails", Index = 10)]
    public string Emails { get; set; } = string.Empty;

    [CsvColumn("Websites", Index = 11)]
    public string Websites { get; set; } = string.Empty;
}
