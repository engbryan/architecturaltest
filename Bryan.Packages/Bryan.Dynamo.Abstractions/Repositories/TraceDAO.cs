using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Bryan.Dynamo.Abstractions.Entities;
using Bryan.Dynamo.Abstractions.Interfaces;
using Bryan.Proof.Common.Constants;
using Bryan.Proof.Domain.Model;
using Bryan.Proof.Domain.Repository;
using System.Text.Json;

namespace Bryan.Dynamo.Abstractions.Repositories;

public class TraceDAO : AbstractDynamoDAO<TraceRecord, string>, ITraceRepository
{
    private const string TABLE_NAME = DynamoConstants.TRACE_TABLE;
    private readonly IAmazonDynamoDB _dynamoClient;

    public TraceDAO(IAmazonDynamoDB dynamoClient)
        : base(dynamoClient, TABLE_NAME) => _dynamoClient = dynamoClient;

    public override IList<TraceRecord> FindAllByPK(string pk)
    {
        var attributeValues = new Dictionary<string, AttributeValue>
        {
            { ":v_pk", new AttributeValue { S = pk } },
        };

        var expressionAttributeNames = new Dictionary<string, string>
        {
            { "#v_reference", "Reference" }, //Reserved Keyword
        };

        var queryRequest = new QueryRequest
        {
            TableName = TABLE_NAME,
            KeyConditionExpression = "#v_reference = :v_pk",
            ExpressionAttributeValues = attributeValues,
            ExpressionAttributeNames = expressionAttributeNames,
            ConsistentRead = ConsistentRead,
            Select = Select.ALL_ATTRIBUTES,
        };

        var itemList = new List<Dictionary<string, AttributeValue>>();
        Dictionary<string, AttributeValue> lastEvaluatedKey;
        do
        {
            var result = _dynamoClient.QueryAsync(queryRequest).GetAwaiter().GetResult();
            itemList.AddRange(result.Items);
            lastEvaluatedKey = result.LastEvaluatedKey;
            queryRequest.ExclusiveStartKey = lastEvaluatedKey;
        }
        while (lastEvaluatedKey.Count > 0);

        var list = new List<TraceRecord>();
        foreach (var record in itemList)
        {
            var dynamoDoc = Document.FromAttributeMap(record);
            list.Add(JsonSerializer.Deserialize<TraceRecord>(dynamoDoc.ToJson())!);
        }

        return list;
    }
}