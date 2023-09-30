using Bryan.Dynamo.Abstractions.Abstracts.Entities;

namespace Bryan.Dynamo.Abstractions.Interfaces;

public interface ITraceRepository : IDynamoRepository<TraceRecord, string>
{
}