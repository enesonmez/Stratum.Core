namespace Core.Persistence.Abstractions.Repositories;

public interface IQuery<out T>
{
    IQueryable<T> Query();
}