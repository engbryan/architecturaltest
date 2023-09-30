using Amazon.DynamoDBv2;
using Bryan.TokenAuth.Config;
using Bryan.TokenAuth.Entities;
using Bryan.TokenAuth.Repositories;

namespace Bryan.Proof.Auth.Domain.Repositories;

public class MarketTypeRepository : MarketTypeDAO
{
    protected override string TableName => "sh-domain";

    public MarketTypeRepository(IAmazonDynamoDB dynamoClient, ILogger<MarketType> logger, SecurityConfig securityConfig)
        : base(dynamoClient, logger, securityConfig)
    {
    }
}