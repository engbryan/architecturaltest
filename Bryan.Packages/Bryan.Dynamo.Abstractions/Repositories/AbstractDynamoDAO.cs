using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Bryan.Dynamo.Abstractions.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Bryan.Dynamo.Abstractions.Repositories;

[ExcludeFromCodeCoverage]
public abstract class AbstractDynamoDAO<T, TId> : IDynamoRepository<T, TId>
    where T : class, IEntity<T>
{
    private readonly IAmazonDynamoDB _dynamoClient;
    private readonly string _tableName;

    public bool ConsistentRead { get; set; }

    protected AbstractDynamoDAO(IAmazonDynamoDB dynamoClient, string tableName)
    {
        _dynamoClient = dynamoClient;
        _tableName = tableName;
        ConsistentRead = false;
    }

    public T? FindByKeys(TId PK, string? SK)
    {
        if (string.IsNullOrWhiteSpace(PK?.ToString()))
            return null;

        var currentTable = Table.LoadTable(_dynamoClient, _tableName);

        var result = new Document();
        result = SK == null
            ? currentTable.GetItemAsync(PK.ToString(), new GetItemOperationConfig() { ConsistentRead = ConsistentRead }).GetAwaiter().GetResult()
            : currentTable.GetItemAsync(PK.ToString(), SK, new GetItemOperationConfig() { ConsistentRead = ConsistentRead }).GetAwaiter().GetResult();

        if (result == null)
            return null;
        else
            return JsonSerializer.Deserialize<T>(result.ToJson());
    }

    public void Save(T entity)
    {
        var currentTable = Table.LoadTable(_dynamoClient, _tableName);
        currentTable.PutItemAsync(Document.FromJson(JsonSerializer.Serialize(entity))).GetAwaiter().GetResult();
    }

    public void Save(IList<T> entity)
    {
        var currentTable = Table.LoadTable(_dynamoClient, _tableName);
        var documentList = entity.ToList().ConvertAll(item => Document.FromJson(JsonSerializer.Serialize(item)));
        var batchWrite = currentTable.CreateBatchWrite();

        var batchLimiter = 0;

        foreach (var item in documentList)
        {
            //BatchWrite tem limite de 25 requests por batch.
            if (batchLimiter < 25)
            {
                batchWrite.AddDocumentToPut(item);
                batchLimiter++;
            }
            else
            {
                //Envia os documentos num BatchWrite
                batchWrite
                    .ExecuteAsync()
                    .GetAwaiter()
                    .GetResult();

                //Instanciando novo objeto para limpar os documentos no BatchWrite após enviar para DynamoDB.
                batchWrite = currentTable.CreateBatchWrite();
                batchWrite.AddDocumentToPut(item);
                batchLimiter = 0;
            }
        }

        //Se sobrou algum item no BatchWrite, executa o envio pela última vez.
        if (batchLimiter > 0)
        {
            batchWrite
                .ExecuteAsync()
                .GetAwaiter()
                .GetResult();
        }
    }

    public void Update(T entity)
    {
        var currentTable = Table.LoadTable(_dynamoClient, _tableName);
        currentTable.UpdateItemAsync(Document.FromJson(JsonSerializer.Serialize(entity))).GetAwaiter().GetResult();
    }

    public void Delete(T entity)
    {
        var currentTable = Table.LoadTable(_dynamoClient, _tableName);

        currentTable.DeleteItemAsync(Document.FromJson(JsonSerializer.Serialize(entity))).GetAwaiter().GetResult();
    }

    public IList<T> FindByKeysList(IList<(string pk, string sk)> keyList)
    {
        var currentTable = Table.LoadTable(_dynamoClient, _tableName);

        var batchGet = currentTable.CreateBatchGet();
        batchGet.ConsistentRead = ConsistentRead;

        foreach (var (pk, sk) in keyList)
        {
            batchGet.AddKey(pk, sk);
        }

        batchGet.ExecuteAsync().GetAwaiter().GetResult();

        var list = new List<T>();
        foreach (var doc in batchGet.Results)
            list.Add(JsonSerializer.Deserialize<T>(doc.ToJson())!);

        return list;
    }

    public virtual IList<T> FindAllByPK(TId PK)
    {
        var attributeValues = new Dictionary<string, AttributeValue>
        {
            { ":v_pk", new AttributeValue { S = PK!.ToString() } },
        };

        var queryRequest = new QueryRequest
        {
            TableName = _tableName,
            KeyConditionExpression = "PK = :v_pk",
            ExpressionAttributeValues = attributeValues,
            ConsistentRead = ConsistentRead,
            Select = Select.ALL_ATTRIBUTES,
        };

        var itemList = new List<Dictionary<string, AttributeValue>>();
        Dictionary<string, AttributeValue> lastEvaluatedKey = null;
        do
        {
            var result = _dynamoClient.QueryAsync(queryRequest).GetAwaiter().GetResult();
            itemList.AddRange(result.Items);
            lastEvaluatedKey = result.LastEvaluatedKey;
            queryRequest.ExclusiveStartKey = lastEvaluatedKey;
        }
        while (lastEvaluatedKey.Count > 0);

        var list = new List<T>();
        foreach (var record in itemList)
        {
            var dynamoDoc = Document.FromAttributeMap(record);
            list.Add(JsonSerializer.Deserialize<T>(dynamoDoc.ToJson())!);
        }

        return list;
    }
}