using System.Linq.Expressions;
using Core.Localization.DB.Entities;
using Core.Persistence.Abstractions.Repositories;

namespace Core.Localization.DB.Repositories.Resources;

public interface IResourceReadRepository : IAsyncReadRepository<Resource,int>, IReadRepository<Resource, int>
{
    Task<Resource?> GetResourceWithTranslationAsync(
        Expression<Func<Resource, bool>> predicate,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
}