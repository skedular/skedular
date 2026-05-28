using HotChocolate;

namespace Team.Api.Models;

[GraphQLName("TeamPatchField")]
public enum TeamPatchField
{
    Name,
    About,
    PrimaryLocation,
    Timezone,
    FeatureImages
}

[GraphQLName("TeamMembersPatchField")]
public enum TeamMembersPatchField
{
    Members
}

[GraphQLName("TeamAndMembersPatchField")]
public enum TeamAndMembersPatchField
{
    Team,
    Members
}

public record TeamPatchRequest(
    Shared.Models.Team Team,
    IReadOnlySet<TeamPatchField> FieldsToUpdate);

public record TeamAndMembersPatchRequest(
    Shared.Models.Team Team,
    IReadOnlySet<TeamAndMembersPatchField> FieldsToUpdate);
