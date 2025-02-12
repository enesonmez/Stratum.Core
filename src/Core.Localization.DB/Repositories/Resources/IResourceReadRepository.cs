using Core.Localization.DB.Entities;
using Core.Persistence.Repositories;

namespace Core.Localization.DB.Repositories.Resources;

public interface IResourceReadRepository : IAsyncReadRepository<Resource,int>, IReadRepository<Resource, int>
{
    
}