using Hermes;
using Hermes.Mappers;

namespace Bryan.TokenAuth.Dynamo.Entities;

public class MarketTypeEntityMap : ClassMap<MarketTypeEntity>
{
    public MarketTypeEntityMap()
    {
        TableName("sh-domain");
        KeySchema(KeySchemaEnum.Simple);

        Map(u => u.PK).WithHash("PK");
        Map(u => u.SK).WithRange("SK"); //.And(new SpecificRange("mainSchema"));

        HasMany(u => u.ApiList);
        HasMany(u => u.RoleList);
    }
}