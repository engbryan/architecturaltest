namespace Bryan.Dynamo.Abstractions.Interfaces;

public interface IDomainRepository<T>
    where T : class, IEntity<T>
{
    Task<Result> Delete(string code);

    Task<Result<List<T>>> GetAll();

    Task<Result<List<T>>> GetAllAsync();

    Task<Result<T?>> GetById(string code);

    Task<Result<T>> GetByIdAsync(string code);

    Task<Result> Save(T domain);

    Task<Result> Save(IList<T> entity);

    Task<Result> SaveAsync(T domain);
}