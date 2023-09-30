using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Bryan.Dynamo.Abstractions.Interfaces;
using Bryan.Dynamo.Abstractions.Repositories;
using System.Text.Json;

namespace Bryan.Proof.Auth.Domain.Repositories;

public interface ITraceRepository : IDynamoRepository<TraceRecord, string>
{
}

public class TraceRepository : AbstractDynamoDAO<TraceRecord, string>, ITraceRepository
{
    private const string TABLE_NAME = "sh-trace";
    private readonly IAmazonDynamoDB _dynamoClient;

    public TraceRepository(IAmazonDynamoDB dynamoClient)
        : base(dynamoClient, TABLE_NAME) => _dynamoClient = dynamoClient;
}