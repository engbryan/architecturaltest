using Hermes;
using Hermes.Mappers;
using SignatureContext.Persistence.Entities;

namespace SignatureContext.Persistence.Configuration
{
    public class SignerEntityMap : ClassMap<SignerEntity>
    {
        public SignerEntityMap()
        {
            Map(u => u.PK).WithHash("PK");
            Map(u => u.SK).WithRange("SK").And(new MergeRange("Signer"));
            Map(u => u.Cge);
            Map(u => u.Name);
            Map(u => u.Email);
        }
    }
}
