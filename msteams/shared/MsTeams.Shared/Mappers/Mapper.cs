using Microsoft.Graph.Models;
using MsTeams.Shared.Models;
using AzureTenant = MsTeams.Shared.Database.Entities.AzureTenant;
using Team = Microsoft.Graph.Models.Team;

namespace MsTeams.Shared.Mappers;

public interface IMapper
{
    AzureTenantTeam MapTo(Team src);
    AzureTenantTeamChannel MapTo(Channel src);
    Database.Entities.AzureTenantTeam MapTo(AzureTenantTeam src, AzureTenant azureTenant);
    Database.Entities.AzureTenantTeam MergeToEntity(AzureTenantTeam src, Database.Entities.AzureTenantTeam dest, AzureTenant azureTenant);

    Database.Entities.AzureTenantTeamChannel MapTo(AzureTenantTeamChannel src, Database.Entities.AzureTenantTeam azureTenantTeam);

    Database.Entities.AzureTenantTeamChannel MergeToEntity(
        AzureTenantTeamChannel src,
        Database.Entities.AzureTenantTeamChannel dest,
        Database.Entities.AzureTenantTeam azureTenantTeam);
}

public class Mapper : IMapper
{
    public AzureTenantTeam MapTo(Team src) =>
        new() { Id = src.Id!, Name = src.DisplayName!, Description = src.Description!, WebUrl = src.WebUrl! };

    public AzureTenantTeamChannel MapTo(Channel src) =>
        new()
        {
            Id = src.Id!,
            Name = src.DisplayName!,
            Description = src.Description!,
            WebUrl = src.WebUrl!,
            Email = src.Email!
        };

    public Database.Entities.AzureTenantTeam MapTo(AzureTenantTeam src, AzureTenant azureTenant) =>
        MergeToEntity(src, new Database.Entities.AzureTenantTeam(), azureTenant);

    public Database.Entities.AzureTenantTeam MergeToEntity(AzureTenantTeam src, Database.Entities.AzureTenantTeam dest, AzureTenant azureTenant)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.WebUrl = src.WebUrl;
        dest.AzureTenant = azureTenant;
        return dest;
    }

    public Database.Entities.AzureTenantTeamChannel MapTo(AzureTenantTeamChannel src, Database.Entities.AzureTenantTeam azureTenantTeam) =>
        MergeToEntity(src, new Database.Entities.AzureTenantTeamChannel(), azureTenantTeam);

    public Database.Entities.AzureTenantTeamChannel MergeToEntity(
        AzureTenantTeamChannel src,
        Database.Entities.AzureTenantTeamChannel dest,
        Database.Entities.AzureTenantTeam azureTenantTeam)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.WebUrl = src.WebUrl;
        dest.Email = src.Email;
        dest.AzureTenantTeam = azureTenantTeam;
        return dest;
    }
}
