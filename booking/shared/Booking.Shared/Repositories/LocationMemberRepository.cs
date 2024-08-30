using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;

namespace Booking.Shared.Repositories;

public interface ILocationMemberRepository : IRepository<LocationMember>
{
    LocationMember Add(LocationMember locationMember);
    LocationMember Update(LocationMember locationMember);
    void RemoveRange(ICollection<LocationMember> locationMembers);
}

public class LocationMemberRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, LocationMember>(dbContext), ILocationMemberRepository
{
    public LocationMember Add(LocationMember locationMember)
    {
        var now = timeProvider.GetUtcNow();
        locationMember.CreatedAt = now;
        return DbContext.LocationMember.Add(locationMember).Entity;
    }

    public void RemoveRange(ICollection<LocationMember> locationMembers)
    {
        var now = timeProvider.GetUtcNow();
        locationMembers.ForEach(locationMember => locationMember.DeletedAt = now);
        DbContext.LocationMember.UpdateRange(locationMembers);
    }

    public LocationMember Update(LocationMember locationMember)
    {
        var now = timeProvider.GetUtcNow();
        locationMember.ModifiedAt = now;
        return DbContext.LocationMember.Update(locationMember).Entity;
    }
}
