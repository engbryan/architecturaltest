namespace SignatureContext.Persistence.Entities
{
    public class SignerEntity : Signer
    {
        public virtual string PK { get; private set; }

        public virtual string SK { get; private set; }

        public SignerEntity(Signer signer) : base(signer)
        {
        }

        public SignerEntity(string documentId, Signer signer) : base(signer)
        {
            PK = documentId;
            SK = signer.Id.ToString();
        }

        public SignerEntity(SignerEntity signer) : base(signer)
        {
            var guid = signer.SK[(signer.SK.IndexOf("-") + 1)..];
            base.Id = Guid.Parse(guid);
        }

        public SignerEntity()
        { }
    }
}
