namespace Enterprise.Shared.Database;

public abstract class RepositoryBase<TContext, TEntity>(TContext dbContext)
    : IRepository<TEntity>
    where TContext : DbContextBase<TContext>
    where TEntity : EntityBase
{
    protected readonly TContext DbContext = dbContext;

    public IUnitOfWork UnitOfWork => DbContext;

    public virtual IQueryable<TEntity> Query(ISpecification<TEntity>? specification = null) =>
        ApplySpecification(specification);

    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity>? spec) =>
        SpecificationEvaluator<TEntity>.GetQuery(DbContext.Set<TEntity>().AsQueryable(), spec);
}
