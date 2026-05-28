namespace Enterprise.Shared.Database;

public interface IRepository<TEntity> where TEntity : class
{
    IUnitOfWork UnitOfWork { get; }
}
