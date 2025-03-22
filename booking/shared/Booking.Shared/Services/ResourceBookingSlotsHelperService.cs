using Booking.Shared.Database.Entities;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Services;

public interface IResourceBookingSlotsHelperService
{
    Task GenerateAllAsync(CancellationToken cancellationToken);
}

public class ResourceBookingSlotsHelperService(IBookingInternalPublisher bookingInternalPublisher, IRepositoryFactory repositoryFactory)
    : IResourceBookingSlotsHelperService
{
    public async Task GenerateAllAsync(CancellationToken cancellationToken)
    {
        var resourceIds = await repositoryFactory.ResourceRepository.Query(
                new Specification<Resource> { Criteria = query => !query.DeletedAt.HasValue })
            .Select(item => item.Id)
            .ToListAsync(cancellationToken);

        await bookingInternalPublisher.PublishGenerateResourceBookingSlotAsync(resourceIds, cancellationToken);
    }
}
