using Core.Localization.DB.Entities;
using Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Localization.DB.Repositories.Resources;

public class ResourceReadRepository<TContext> : EfReadRepositoryBase<Resource, int, TContext>, IResourceReadRepository
    where TContext : DbContext
{
    public ResourceReadRepository(TContext context)
        : base(context) { }
}