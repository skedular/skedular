using Microsoft.Graph.Models;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Mappers;

public interface IMapper
{
    TenantMember MapToEntity(User src);
}

public class Mapper : IMapper
{
    public TenantMember MapToEntity(User src) =>
        new()
        {
            Id = src.Id,
            GivenName = src.GivenName,
            Surname = src.Surname,
            JobTitle = src.JobTitle,
            Email = src.Mail,
            PrincipalName = src.UserPrincipalName,
            PreferredLanguage = src.PreferredLanguage
        };
}
