namespace Core.Persistence.Repositories;

public interface IWriteRepository<TEntity, TEntityId> : IQuery<TEntity>
    where TEntity : Entity<TEntityId>
{
    TEntity Add(TEntity entity);

    ICollection<TEntity> AddRange(ICollection<TEntity> entities);

    TEntity Update(TEntity entity);

    ICollection<TEntity> UpdateRange(ICollection<TEntity> entities);

    TEntity Delete(TEntity entity, bool permanent = false);

    ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false);
}