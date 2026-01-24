using Core.Persistence.Abstractions.Repositories;

namespace Core.Security.Abstractions.Entities;

public class OperationClaim<TId> : Entity<TId>
{
    public string Name { get; protected set; }

    public OperationClaim()
    {
        Name = string.Empty;
    }
    
    public OperationClaim(string name)
    {
        Name = name;
    }

    public OperationClaim(TId id, string name)
        : base(id)
    {
        Name = name;
    }
}