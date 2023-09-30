using Amazon.DynamoDBv2;
using Hermes;
using Hermes.Configuration;
using SignatureContext.Persistence.Entities;
using System.Reflection;

namespace SignatureContext.Persistence
{
    public class SignatureDbContext : DynamoDbStore<DocumentEntity, Guid>
    {                                               
        public SignatureDbContext(IAmazonDynamoDB client) : base(client)
        {
            //Operation = new(client);
        }

        //public DynamoDbStore<OperationEntity, Guid> Operation;
    }
}
