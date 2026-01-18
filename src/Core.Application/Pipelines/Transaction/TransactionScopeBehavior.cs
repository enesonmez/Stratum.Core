using System.Transactions;
using Core.Persistence.Abstractions.UnitOfWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Pipelines.Transaction;

public class TransactionScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionScopeBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var unitOfWorks = _serviceProvider.GetServices<IUnitOfWork>();
        TResponse response;

        try
        {
            response = await next();

            foreach (var unitOfWork in unitOfWorks)
                await unitOfWork.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();
        }
        catch (Exception)
        {
            transactionScope.Dispose();
            throw;
        }

        return response;
    }
}