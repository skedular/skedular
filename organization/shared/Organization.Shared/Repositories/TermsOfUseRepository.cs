using Enterprise.Shared.Database;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface ITermsOfUseRepository : IRepository<TermsOfUse>;

public class TermsOfUseRepository(OrganizationDbContext dbContext)
    : RepositoryBase<OrganizationDbContext, TermsOfUse>(dbContext), ITermsOfUseRepository;
