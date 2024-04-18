namespace Enterprise.Shared.Database;

public interface IRepository<TEntity> where TEntity : class
{
    IUnitOfWork UnitOfWork { get; }
    IQueryable<TEntity> Query(ISpecification<TEntity>? specification = null);
}
