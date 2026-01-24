using Core.Persistence.Abstractions.Repositories;

namespace Core.Security.Abstractions.Entities;

public class UserOperationClaim<TId, TUserId, TOperationClaimId> : Entity<TId>
{
    public TUserId UserId { get; protected set; }
    public TOperationClaimId OperationClaimId { get; protected set; }

    public UserOperationClaim()
    {
        UserId = default!;
        OperationClaimId = default!;
    }

    public UserOperationClaim(TUserId userId, TOperationClaimId operationClaimId)
    {
        UserId = userId;
        OperationClaimId = operationClaimId;
    }

    public UserOperationClaim(TId id, TUserId userId, TOperationClaimId operationClaimId)
        : base(id)
    {
        UserId = userId;
        OperationClaimId = operationClaimId;
    }
}