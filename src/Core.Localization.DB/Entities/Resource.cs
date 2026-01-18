using Core.Persistence.Abstractions.Repositories;

namespace Core.Localization.DB.Entities;

public class Resource : Entity<int>
{
    public required string Key { get; set; }
    public ICollection<ResourceTranslation> ResourceTranslations { get; } = new List<ResourceTranslation>();
}