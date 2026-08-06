using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Google.Protobuf.WellKnownTypes;
using Marketplace.Processors.Mappers;

namespace Marketplace.Processors.UnitTests.Mappers.EventMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapOrganizationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Project_Spaces_Trial_Inputs(
        EventMapper sut,
        string organizationId,
        string offeringId,
        DateTimeOffset trialStartedAt)
    {
        var trialEndsAt = trialStartedAt.AddDays(14);
        var nextBillingAt = trialStartedAt.AddMonths(1);
        var @event = new Event
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

        var result = sut.MapTo(@event).Offering!;

        result.SpacesProductEnabled.ShouldBe(true);
        result.SpacesTrialStartedAt.ShouldBe(trialStartedAt);
        result.SpacesTrialEndsAt.ShouldBe(trialEndsAt);
        result.SpacesNextBillingAt.ShouldBe(nextBillingAt);
    }
}
