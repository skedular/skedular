using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IIndustrySubCategoryRepository : IRepository<IndustrySubCategory>;

public class IndustrySubCategoryRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, IndustrySubCategory>(dbContext, timeProvider),
        IIndustrySubCategoryRepository;
