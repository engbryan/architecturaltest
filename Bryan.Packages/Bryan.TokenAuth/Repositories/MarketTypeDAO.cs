using Amazon.DynamoDBv2;
using Bryan.Dynamo.Abstractions.Repositories;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.TokenAuth.Repositories;

[ExcludeFromCodeCoverage]
public class MarketTypeDAO : AbstractDomainDAO<MarketType>, IMarketRepository
{
    private readonly SecurityConfig.ClaimConfig _claimConfig;

    public MarketTypeDAO(
        IAmazonDynamoDB dynamoClient,
        ILogger<MarketType> logger,
        SecurityConfig securityConfig
    )
        : base(logger, dynamoClient) => _claimConfig = securityConfig.Claim;

    public async Task<Result<MarketType?>> GetByGuid(string guid)
    {
        var (_, table, tableError) = await GetAllAsync();
        if (table is null)
            return tableError!;

        var market = table
            .FirstOrDefault(m => m.Guid.ToString()
            .Equals(guid, StringComparison.InvariantCultureIgnoreCase));

        if (market is null)
            return new ResultException($"MarketType {guid} not found");

        return market!;
    }
}