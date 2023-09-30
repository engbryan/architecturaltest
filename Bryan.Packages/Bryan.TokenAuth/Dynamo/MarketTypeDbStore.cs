using Amazon.DynamoDBv2;
using Hermes;

namespace Bryan.TokenAuth;

public class MarketTypeDbStore : DynamoDbStore<MarketTypeEntity, Guid>
{
    public MarketTypeDbStore(IAmazonDynamoDB client)
        : base(client)
    {
        //Operation = new(client);
    }

    //public DynamoDbStore<OperationEntity, Guid> Operation;
}