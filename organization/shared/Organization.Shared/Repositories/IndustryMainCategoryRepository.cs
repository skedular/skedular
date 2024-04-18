using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IIndustryMainCategoryRepository : IRepository<IndustryMainCategory>;

public class IndustryMainCategoryRepository(OrganizationDbContext dbContext)
    : RepositoryBase<OrganizationDbContext, IndustryMainCategory>(dbContext), IIndustryMainCategoryRepository;
