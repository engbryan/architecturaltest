using Bryan.Dynamo.Abstractions.Interfaces;

namespace Bryan.TokenAuth.Repositories;

public interface IMarketRepository : IDomainRepository<MarketType>
{
    Task<Result<MarketType?>> GetByGuid(string guid);
}