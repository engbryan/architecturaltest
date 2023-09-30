using Bryan.Proof.Auth.Domain.Model.StaticData;

namespace Bryan.TokenAuth.Entities;

public class MarketTypeEntity : MarketTypeNew
{
    public virtual string PK { get; private set; }

    public virtual string SK { get; private set; }

    public MarketTypeEntity(MarketTypeNew marketType)
        : base(marketType) => PK = marketType.Guid.ToString();

    public MarketTypeEntity(MarketTypeEntity document)
        : base(document) => Id = Guid.Parse(document.PK);

    public MarketTypeEntity()
    {
    }
}