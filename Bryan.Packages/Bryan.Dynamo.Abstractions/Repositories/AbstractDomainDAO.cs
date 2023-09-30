using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Bryan.Dynamo.Abstractions.Abstracts.Entities;
using Bryan.Dynamo.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Bryan.Dynamo.Abstractions.Repositories;

[ExcludeFromCodeCoverage]
public abstract class AbstractDomainDAO<T> : IDomainRepository<T>
    where T : AbstractDomain<T>, IEntity<T>, new()
{
    protected virtual string TableName => default!;
    //private readonly string _tableName = DynamoConstants.DOMAIN_TABLE;

    protected readonly ILogger<T> _logger;
    protected readonly IAmazonDynamoDB _dynamoClient;

    protected AbstractDomainDAO(
        ILogger<T> logger,
        IAmazonDynamoDB dynamoClient
    )
    {
        if (string.IsNullOrEmpty(TableName))
            throw new Exception($"You need do implement: protected override string {nameof(TableName)} => \"Table'sName\"!");

        _dynamoClient = dynamoClient;
        _logger = logger;
    }

    public async Task<Result> Delete(string code)
    {
        var (_, table, tableError) = LoadTableInternal();
        if (table is null)
            return tableError!;

        var entity = GetById(code);

        await table.DeleteItemAsync(Document.FromJson(JsonSerializer.Serialize(entity)));

        return Result.Success();
    }

    public async virtual Task<Result<List<T>>> GetAll()
    {
        var (_, table, tableError) = LoadTableInternal();
        if (table is null)
            return tableError!;

        var group = new T().Group;

        var config = new QueryOperationConfig()
        {
            Select = SelectValues.AllAttributes,
            KeyExpression = new Expression
            {
                ExpressionAttributeNames = new Dictionary<string, string> { { "#PK", "Group" } },
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        {
                            ":pk", group
                        }
                    },
                ExpressionStatement = "#PK = :pk "
            },
        };

        var query = table.Query(config);
        var resultList = new List<Document>();
        do
        {
            var result = await query.GetNextSetAsync();
            if (result != null)
                resultList.AddRange(result);
        }
        while (!query.IsDone);

        if (resultList.Any())
            return resultList.ConvertAll(r => JsonSerializer.Deserialize<T>(r.ToJson()))!;

        return new List<T>();
    }

    public async Task<Result<List<T>>> GetAllAsync()
    {
        var (_, table, tableError) = LoadTableInternal();
        if (table is null)
            return tableError!;

        var group = new T().Group;

        var config = new QueryOperationConfig()
        {
            Select = SelectValues.AllAttributes,
            KeyExpression = new Expression
            {
                ExpressionAttributeNames = new Dictionary<string, string> { { "#PK", "Group" } },
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        {
                            ":pk", group
                        }
                    },
                ExpressionStatement = "#PK = :pk "
            },
        };

        var query = table.Query(config);
        var resultList = new List<Document>();
        do
        {
            var result = await query.GetNextSetAsync();
            if (result != null)
                resultList.AddRange(result);
        }
        while (!query.IsDone);

        return resultList.Any() ? resultList.ConvertAll(r => JsonSerializer.Deserialize<T>(r.ToJson()))! : new List<T>();
    }

    public async virtual Task<Result<T?>> GetById(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return default;

        var (_, table, tableError) = LoadTableInternal();
        if (table is null)
            return tableError!;

        var obj = new T();
        var result = await table.GetItemAsync(obj.Group, code);
        if (result == null)
            return default;
        else
            return JsonSerializer.Deserialize<T>(result.ToJson())!;
    }

    public async Task<Result<T>> GetByIdAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return new Exception($"AbstractDomainDAO<{typeof(T).Name}>.GetByIdAsync code must not be empty");

        var (_, table, tableError) = LoadTableInternal();
        if (table is null)
            return tableError!;

        var obj = new T();
        var result = await table.GetItemAsync(obj.Group, code);
        if (result == null)
            return default!;
        else
            return JsonSerializer.Deserialize<T>(result.ToJson())!;
    }

    public async Task<Result> Save(T domain)
    {
        var (_, table, tableError) = LoadTableInternal();
        if (table is null)
            return tableError!;

        var document = Document.FromJson(JsonSerializer.Serialize(domain));

        await table.PutItemAsync(document);

        return Result.Success();
    }

    public async Task<Result> Save(IList<T> entity)
    {
        var (_, table, tableError) = LoadTableInternal();
        if (table is null)
            return tableError!;

        var documentList = entity.ToList().ConvertAll(item => Document.FromJson(JsonSerializer.Serialize(item)));
        var batchWrite = table.CreateBatchWrite();

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
                await batchWrite.ExecuteAsync();

                //Instanciando novo objeto para limpar os documentos no BatchWrite após enviar para DynamoDB.
                batchWrite = table.CreateBatchWrite();
                batchWrite.AddDocumentToPut(item);
                batchLimiter = 0;
            }
        }

        //Se sobrou algum item no BatchWrite, executa o envio pela última vez.
        if (batchLimiter > 0)
        {
            await batchWrite.ExecuteAsync();
        }

        return Result.Success();
    }

    public async Task<Result> SaveAsync(T domain)
    {
        var (_, table, tableError) = LoadTableInternal();
        if (table is null)
            return tableError!;

        var document = Document.FromJson(JsonSerializer.Serialize(domain));

        await table.PutItemAsync(document);

        return Result.Success();
    }

    private Result<Table> LoadTableInternal()
    {
        try
        {
            return LoadTable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, typeof(AbstractDomainDAO<T>).Name);
            return new Exception($"Could not connect to amazon table: '{TableName}'. {ex.Message}");
        }
    }

    protected virtual Result<Table> LoadTable() => Table.LoadTable(_dynamoClient, TableName);
}