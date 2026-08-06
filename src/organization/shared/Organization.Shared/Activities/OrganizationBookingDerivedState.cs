using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Organization.Shared.Database.Entities;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using Temporalio.Activities;

namespace Organization.Shared.Activities;

public class OrganizationBookingDerivedState(
    IRepositoryFactory repositoryFactory,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    IRandomHelper randomHelper,
    ICachedOrganizationService cachedOrganizationService,
    IOrganizationPublisher organizationPublisher,
    IEntityMapper entityMapper)
{
    [Activity]
    public async Task RecomputeAsync(string organizationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken);
        if (organization is null || organization.IsDeleted())
        {
            return;
        }

        var allBookings = await GetBookingsAsync(
            new BookingWhereInput
            {
                OrganizationId = organizationId,
            },
            cancellationToken);

        await ReplaceDailyBookingCountsAsync(organization, allBookings, cancellationToken);
        var activeMembersChanged = await ReplaceActiveMembersAsync(organization, cancellationToken);

        _ = repositoryFactory.OrganizationRepository.Update(organization);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        if (activeMembersChanged)
        {
            await organizationPublisher.PublishOrganizationsAsync([entityMapper.MapTo(organization)], cancellationToken);
        }
    }

    private async Task ReplaceDailyBookingCountsAsync(
        Database.Entities.Organization organization,
        List<BookingSnapshot> bookings,
        CancellationToken cancellationToken)
    {
        var recordings = bookings
            .GroupBy(item => item.From.StartOfDay())
            .Select(groupedBooking => new DailyBookingCountRecording
            {
                Id = randomHelper.Generate(),
                Date = groupedBooking.Key,
                Count = groupedBooking.Count(),
                Organization = organization,
            })
            .ToList();

        await repositoryFactory.DailyBookingCountRecordingRepository.ReplaceDailyRecordingsAsync(
            organization.Id,
            recordings,
            cancellationToken);
    }

    private async Task<bool> ReplaceActiveMembersAsync(
        Database.Entities.Organization organization,
        CancellationToken cancellationToken)
    {
        var organizationOffering = organization.OrganizationOfferings
            .Where(item => item.IsNotDeleted())
            .OrderByDescending(item => item.End)
            .FirstOrDefault();
        if (organizationOffering is null)
        {
            return false;
        }

        var existingActiveMembers = await repositoryFactory.OrganizationOfferingActiveMemberRepository
            .GetByOfferingIdAsync(organizationOffering.Id, cancellationToken);

        var existingOrganizationMemberIds = existingActiveMembers
            .Select(item => item.OrganizationMember.Id)
            .Distinct()
            .Order()
            .ToList();

        var bookedCustomerIds = (await GetBookingsAsync(
                new BookingWhereInput
                {
                    OrganizationId = organization.Id,
                    FromGte = Timestamp.FromDateTimeOffset(organizationOffering.Start),
                    ToLte = Timestamp.FromDateTimeOffset(organizationOffering.End),
                },
                cancellationToken))
            .SelectMany(item => item.CustomerIds)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct()
            .ToHashSet();

        var nextOrganizationMembers = organization.OrganizationMembers
            .Where(item => item.IsNotDeleted() && bookedCustomerIds.Contains(item.Customer.Id))
            .ToList();

        var nextActiveMembers = nextOrganizationMembers
            .Select(organizationMember => new OrganizationOfferingActiveMember
            {
                Id = randomHelper.Generate(),
                OrganizationMember = organizationMember,
                OrganizationOffering = organizationOffering,
            })
            .ToList();

        await repositoryFactory.OrganizationOfferingActiveMemberRepository.ReplaceAsync(
            existingActiveMembers,
            nextActiveMembers,
            cancellationToken);

        var nextOrganizationMemberIds = nextOrganizationMembers
            .Select(item => item.Id)
            .Distinct()
            .Order()
            .ToList();

        return !existingOrganizationMemberIds.SequenceEqual(nextOrganizationMemberIds);
    }

    private async Task<List<BookingSnapshot>> GetBookingsAsync(BookingWhereInput where, CancellationToken cancellationToken)
    {
        var result = new List<BookingSnapshot>();
        string? after = null;

        do
        {
            var response = await bookingServiceClient.Admin_GetPaginatedBookingsAsync(
                new Admin_GetPaginatedBookingsInput
                {
                    After = after ?? string.Empty,
                    First = 1000,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = where,
                    OrderBy =
                    {
                        new BookingOrderInput
                        {
                            Direction = OrderDirection.Ascending,
                            Field = BookingOrderField.From,
                        },
                    },
                },
                bookingConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);

            result.AddRange(response.Edges.Select(edge => new BookingSnapshot(
                edge.Node.From.ToDateTimeOffset(),
                edge.Node.InvolvedCustomerIds.ToList())));

            after = response.PageInfo.HasNextPage ? response.PageInfo.EndCursor : null;
        } while (!string.IsNullOrWhiteSpace(after));

        return result;
    }

    private sealed record BookingSnapshot(DateTimeOffset From, List<string> CustomerIds);
}
