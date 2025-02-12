using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Core.Persistence.Repositories;

public class EfWriteRepositoryBase<TEntity, TEntityId, TContext>(TContext context)
    : IAsyncWriteRepository<TEntity, TEntityId>,
        IWriteRepository<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TContext : DbContext
{
    private readonly TContext _context = context;

    public IQueryable<TEntity> Query()
    {
        return _context.Set<TEntity>();
    }

    protected virtual void EditEntityPropertiesToAdd(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        EditEntityPropertiesToAdd(entity);
        await _context.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<TEntity>> AddRangeAsync(
        ICollection<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        foreach (TEntity entity in entities)
            EditEntityPropertiesToAdd(entity);
        await _context.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    protected virtual void EditEntityPropertiesToUpdate(TEntity entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        EditEntityPropertiesToUpdate(entity);
        _context.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<TEntity>> UpdateRangeAsync(
        ICollection<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        foreach (TEntity entity in entities)
            EditEntityPropertiesToUpdate(entity);
        _context.UpdateRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false,
        CancellationToken cancellationToken = default)
    {
        await SetEntityAsDeleted(entity, permanent, isAsync: true, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<TEntity>> DeleteRangeAsync(
        ICollection<TEntity> entities,
        bool permanent = false,
        CancellationToken cancellationToken = default
    )
    {
        await SetEntityAsDeleted(entities, permanent, isAsync: true, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public TEntity Add(TEntity entity)
    {
        EditEntityPropertiesToAdd(entity);
        _context.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
    {
        foreach (TEntity entity in entities)
            EditEntityPropertiesToAdd(entity);
        _context.AddRange(entities);
        _context.SaveChanges();
        return entities;
    }

    public TEntity Update(TEntity entity)
    {
        EditEntityPropertiesToAdd(entity);
        _context.Update(entity);
        _context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
    {
        foreach (TEntity entity in entities)
            EditEntityPropertiesToAdd(entity);
        _context.UpdateRange(entities);
        _context.SaveChanges();
        return entities;
    }

    public TEntity Delete(TEntity entity, bool permanent = false)
    {
        SetEntityAsDeleted(entity, permanent, isAsync: false).Wait();
        _context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
    {
        SetEntityAsDeleted(entities, permanent, isAsync: false).Wait();
        _context.SaveChanges();
        return entities;
    }

    private async Task SetEntityAsDeleted(
        TEntity entity,
        bool permanent,
        bool isAsync = true,
        CancellationToken cancellationToken = default
    )
    {
        if (!permanent)
        {
            CheckHasEntityHaveOneToOneRelation(entity);
            if (isAsync)
                await SetEntityAsSoftDeleted(entity, isAsync, cancellationToken);
            else
                await SetEntityAsSoftDeleted(entity, isAsync, cancellationToken).WaitAsync(cancellationToken);
        }
        else
            _context.Remove(entity);
    }

    private async Task SetEntityAsDeleted(
        IEnumerable<TEntity> entities,
        bool permanent,
        bool isAsync = true,
        CancellationToken cancellationToken = default
    )
    {
        foreach (TEntity entity in entities)
            await SetEntityAsDeleted(entity, permanent, isAsync, cancellationToken);
    }

    private void CheckHasEntityHaveOneToOneRelation(TEntity entity)
    {
        var foreignKeys = _context.Entry(entity).Metadata.GetForeignKeys();
        var oneToOneForeignKey = foreignKeys.FirstOrDefault(fk =>
            fk.IsUnique &&
            fk.PrincipalKey.Properties.All(pk => _context.Entry(entity).Property(pk.Name).Metadata.IsPrimaryKey())
        );

        if (oneToOneForeignKey == null) return;
        var relatedEntity = oneToOneForeignKey.PrincipalEntityType.ClrType.Name;
        var primaryKeyProperties = _context.Entry(entity).Metadata.FindPrimaryKey()!.Properties;
        var primaryKeyNames = string.Join(", ", primaryKeyProperties.Select(prop => prop.Name));
        throw new InvalidOperationException(
            $"Entity {entity.GetType().Name} has a one-to-one relationship with {relatedEntity} via the primary key ({primaryKeyNames}). Soft Delete causes problems if you try to create an entry again with the same foreign key."
        );
    }

    private async Task SetEntityAsSoftDeleted(
        IEntityTimestamps entity,
        bool isAsync = true,
        CancellationToken cancellationToken = default,
        bool isRoot = true
    )
    {
        if (IsSoftDeleted(entity))
            return;
        if (isRoot)
            EditEntityPropertiesToDelete((TEntity)entity);
        else
            EditRelationEntityPropertiesToCascadeSoftDelete(entity);

        var navigations = _context
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x =>
                x is
                {
                    IsOnDependent: false,
                    ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade
                }
            )
            .ToList();
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    IQueryable query = _context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();

                    if (isAsync)
                    {
                        IQueryable<object>? relationLoaderQuery = GetRelationLoaderQuery(
                            query,
                            navigationPropertyType: navigation.PropertyInfo.GetType()
                        );
                        if (relationLoaderQuery is not null)
                            navValue = await relationLoaderQuery.ToListAsync(cancellationToken);
                    }
                    else
                        navValue = GetRelationLoaderQuery(query,
                                navigationPropertyType: navigation.PropertyInfo.GetType())
                            ?.ToList();

                    if (navValue == null)
                        continue;
                }

                foreach (object navValueItem in (IEnumerable)navValue)
                    await SetEntityAsSoftDeleted((IEntityTimestamps)navValueItem, isAsync, cancellationToken,
                        isRoot: false);
            }
            else
            {
                if (navValue == null)
                {
                    IQueryable query = _context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();

                    if (isAsync)
                    {
                        IQueryable<object>? relationLoaderQuery = GetRelationLoaderQuery(
                            query,
                            navigationPropertyType: navigation.PropertyInfo.GetType()
                        );
                        if (relationLoaderQuery is not null)
                            navValue = await relationLoaderQuery.FirstOrDefaultAsync(cancellationToken);
                    }
                    else
                        navValue = GetRelationLoaderQuery(query,
                                navigationPropertyType: navigation.PropertyInfo.GetType())
                            ?.FirstOrDefault();

                    if (navValue == null)
                        continue;
                }

                await SetEntityAsSoftDeleted((IEntityTimestamps)navValue, isAsync, cancellationToken, isRoot: false);
            }
        }

        _context.Update(entity);
    }

    protected virtual bool IsSoftDeleted(IEntityTimestamps entity)
    {
        return entity.DeletedDate.HasValue;
    }

    protected virtual void EditEntityPropertiesToDelete(TEntity entity)
    {
        entity.DeletedDate = DateTime.UtcNow;
    }

    protected virtual void EditRelationEntityPropertiesToCascadeSoftDelete(IEntityTimestamps entity)
    {
        entity.DeletedDate = DateTime.UtcNow;
    }

    private IQueryable<object>? GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        Type queryProviderType = query.Provider.GetType();
        MethodInfo createQueryMethod =
            queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
            ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");
        var queryProviderQuery =
            (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: [query.Expression])!;
        return queryProviderQuery.Where(x => !((IEntityTimestamps)x).DeletedDate.HasValue);
    }
}