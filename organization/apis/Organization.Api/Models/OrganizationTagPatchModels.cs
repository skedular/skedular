using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.Models;

[GraphQLName("OrganizationTagPatchField")]
public enum OrganizationTagPatchField
{
    Name,
    Description,
    Color
}

public record OrganizationTagPatchRequest(
    string Id,
    OrganizationTagType Type,
    IReadOnlySet<OrganizationTagPatchField> FieldsToUpdate,
    string? Name,
    string? Description,
    string? Color);
