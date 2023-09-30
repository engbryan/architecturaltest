using Hermes;
using Hermes.Mappers;
using SignatureContext.Domain.Entities;
using SignatureContext.Persistence.Entities;

namespace SignatureContext.Persistence.Configuration
{
    public class DocumentMap : ClassMap<DocumentEntity>
    {
        public DocumentMap()
        {
            TableName("sh-documents-v2");
            KeySchema(KeySchemaEnum.Simple);

            Map(u => u.PK).WithHash("PK");
            Map(u => u.SK).WithRange("SK").And(new SpecificRange("mainSchema"));
            Map(u => u.Contract);
            Map(u => u.Value);

            References(u => u.Market);

            HasMany(u => u.SignersToEntity).And(AccessStrategy.CamelCaseUnderscoreName).Alias("Signer");
            HasMany(u => u.Acts);
        }
    }

    public class MarketMap : ClassMap<Market>
    {
        public MarketMap()
        {
            Map(u => u.Code);
            Map(u => u.Description);
        }
    }
}
