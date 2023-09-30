using SignatureContext.Domain.Entities;

namespace SignatureContext.Persistence.Entities
{
    public class DocumentEntity : Document
    {
        public virtual string PK { get; private set; }

        public virtual string SK { get; private set; }
        
        
        public ICollection<Signer> SignersToDomain
        {
            get => this._signersToEntity.Select(signer =>
            {
                var guid = signer.SK[(signer.SK.IndexOf("-") + 1)..];

                var signerDomain = new Signer(signer);
                signerDomain.SetId(Guid.Parse(guid));

                return new Signer(signerDomain);
            }).ToList();
        }
        public ICollection<SignerEntity> SignersToEntity
        {
            get => base.Signers.Select(signer => new SignerEntity(this.PK, signer)).ToList();
        }

        private readonly IList<SignerEntity> _signersToEntity = new List<SignerEntity>();

        public DocumentEntity(Document document) : base(document)
        {
            PK = document.Id.ToString();
        }

        public DocumentEntity(DocumentEntity document) : base(document)
        {
            base.Id = Guid.Parse(document.PK);
        }

        public DocumentEntity()
        {
        }
    }
}
