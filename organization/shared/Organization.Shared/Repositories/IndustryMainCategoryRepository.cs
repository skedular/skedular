using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IIndustryMainCategoryRepository : IRepository<IndustryMainCategory>;

public class IndustryMainCategoryRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, IndustryMainCategory>(dbContext, timeProvider),
        IIndustryMainCategoryRepository;
