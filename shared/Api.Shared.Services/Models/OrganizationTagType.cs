namespace Api.Shared.Services.Models;

public enum OrganizationTagType
{
    Zone,
    Custom,
    Desk,
    Room
}

public static class OrganizationTagTypeConstants
{
    public const string Zone = "ZONE";
    public const string Custom = "CUSTOM";
    public const string Desk = "DESK";
    public const string Room = "ROOM";
}
