using Bryan.Dynamo.Abstractions.Abstracts.Entities;
using Bryan.Dynamo.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.Dynamo.Abstractions.Repositories;

[ExcludeFromCodeCoverage]
public class RepositoryFactory : IRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RepositoryFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public IDomainRepository<T> GetDomainRepository<T>()
        where T : AbstractDomain<T> => _serviceProvider.GetRequiredService<IDomainRepository<T>>();

    public IDynamoRepository<T, TId> GetDynamoRepository<T, TId>()
        where T : AbstractEntity<T, TId> => _serviceProvider.GetRequiredService<IDynamoRepository<T, TId>>();
}