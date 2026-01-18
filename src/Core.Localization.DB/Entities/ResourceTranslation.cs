using Core.Persistence.Abstractions.Repositories;

namespace Core.Localization.DB.Entities;

public class ResourceTranslation : Entity<int>
{
    public required string CultureCode { get; set; }
    public required string Value { get; set; }
    public int ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;
}