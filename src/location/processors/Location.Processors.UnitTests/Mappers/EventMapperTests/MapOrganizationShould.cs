using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Google.Protobuf.WellKnownTypes;
using Location.Processors.Mappers;

namespace Location.Processors.UnitTests.Mappers.EventMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapOrganizationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Project_Spaces_Trial_Inputs(string organizationId, string offeringId, EventMapper sut)
    {
        var trialStartedAt = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
        var trialEndsAt = trialStartedAt.AddDays(14);
        var nextBillingAt = new DateTimeOffset(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);
        var @event = BuildEvent(organizationId, offeringId, trialStartedAt, trialEndsAt, nextBillingAt);

        var result = sut.MapTo(@event).Offering!;

        result.SpacesProductEnabled.ShouldBe(true);
        result.SpacesTrialStartedAt.ShouldBe(trialStartedAt);
        result.SpacesTrialEndsAt.ShouldBe(trialEndsAt);
        result.SpacesNextBillingAt.ShouldBe(nextBillingAt);
    }

    private static Event BuildEvent(
        string organizationId,
        string offeringId,
        DateTimeOffset trialStartedAt,
        DateTimeOffset trialEndsAt,
        DateTimeOffset nextBillingAt) =>
        new()
        {
            Metadata = new Metadata
            {
                Time = Timestamp.FromDateTimeOffset(trialStartedAt),
            },
            Data = new Data
            {
                Organization = new Organization
                {
                    Id = organizationId,
                    Type = OrganizationType.Marketplace,
                    Offering = new Offering
                    {
                        Id = offeringId,
                        Code = "SPACES_GROWTH_V1",
                        Start = Timestamp.FromDateTimeOffset(trialStartedAt),
                        End = Timestamp.FromDateTimeOffset(nextBillingAt),
                        SpacesProductEnabled = true,
                        SpacesTrialStartedAt = Timestamp.FromDateTimeOffset(trialStartedAt),
                        SpacesTrialEndsAt = Timestamp.FromDateTimeOffset(trialEndsAt),
                        SpacesNextBillingAt = Timestamp.FromDateTimeOffset(nextBillingAt),
                    },
                },
            },
        };
}
