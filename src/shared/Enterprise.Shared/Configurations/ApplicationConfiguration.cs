using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Configurations;

public class ApplicationConfiguration
{
    public const string Key = "Application";

    public string Environment { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string DomainSource { get; set; } = string.Empty;
    public string AppSource { get; set; } = string.Empty;
    public Uri WebAppBaseDomain { get; set; } = Constants.EmptyUri;
    public Uri ApiBaseDomain { get; set; } = Constants.EmptyUri;
    public QuerySplittingBehavior? QuerySplittingBehavior { get; set; }
    public string GetSource() => $"{Environment}::{DomainSource}::{AppSource}";
}
