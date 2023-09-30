using Bryan.Dynamo.Abstractions.Abstracts.Entities;

namespace Bryan.Dynamo.Abstractions.Interfaces;

public interface IRepositoryFactory
{
    IDomainRepository<T> GetDomainRepository<T>()
        where T : AbstractDomain<T>;

    IDynamoRepository<T, TId> GetDynamoRepository<T, TId>()
        where T : AbstractEntity<T, TId>;
}