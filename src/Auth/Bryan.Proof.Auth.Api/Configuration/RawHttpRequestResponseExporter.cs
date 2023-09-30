using Bryan.HttpClients.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.Proof.Auth.Api.Configuration;

[ExcludeFromCodeCoverage]
public class RawHttpRequestResponseExporter : IRawHttpRequestResponseExporter
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RawHttpRequestResponseExporter(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task Export(string rawRequest, string rawResponse, string baseUrl, TimeSpan elapsedTime)
    {
        //using var scope = _scopeFactory.CreateAsyncScope();
        //using var ctx = scope.ServiceProvider.GetRequiredService<DbContext>();
        //await ctx.SaveChangesAsync();
        await Task.CompletedTask;
        await Task.CompletedTask;
    }
}