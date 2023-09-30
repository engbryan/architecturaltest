using Bryan.Dynamo.Abstractions.Abstracts.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.TokenAuth.Entities;

[ExcludeFromCodeCoverage]
public class MarketType : AbstractDomain<MarketType>
{
    public override string Group => nameof(MarketType);

    public virtual List<ApiConfig> ApiList { get; set; }

    public virtual List<AzureRole> RoleList { get; set; }

    public virtual Guid Guid { get; set; }

    public class ApiConfig
    {
        public int Value { get; set; }

        public string Key { get; set; }
    }

    public class AzureRole
    {
        public string Name { get; set; }

        public string Id { get; set; }
    }
}