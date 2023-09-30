namespace Bryan.Dynamo.Abstractions.Interfaces;

public interface IDynamoRepository<T, in TId>
    where T : class, IEntity<T>
{
    T? FindByKeys(TId PK, string? SK);

    IList<T> FindAllByPK(TId PK);

    IList<T> FindByKeysList(IList<(string pk, string sk)> keyList);

    void Save(T entity);

    void Save(IList<T> entity);

    void Update(T entity);

    void Delete(T entity);

    public bool ConsistentRead { get; set; }
}