using System.Linq.Expressions;
using Core.Localization.DB.Entities;
using Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Localization.DB.Repositories.Resources;

public class ResourceReadRepository<TContext> : EfReadRepositoryBase<Resource, int, TContext>, IResourceReadRepository
    where TContext : DbContext
{
    public ResourceReadRepository(TContext context)
        : base(context)
    {
    }

    public async Task<Resource?> GetResourceWithTranslationAsync(Expression<Func<Resource, bool>> predicate,
        bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Resource> queryable = Query().Include(x => x.ResourceTranslations);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }
}