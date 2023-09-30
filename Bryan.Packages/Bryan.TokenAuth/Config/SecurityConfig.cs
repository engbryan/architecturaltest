namespace Bryan.TokenAuth.Config;

public class SecurityConfig
{
    public ActiveDirectoryConfig AD { get; init; } = new();

    public Credential[] Credentials { get; init; }

    //public string HttpHeaderName { get; init; }

    public ClaimConfig Claim { get; init; }

    public class ActiveDirectoryConfig
    {
        public string Instance { get; set; }

        public string TenantId { get; set; }

        public string ApplicationIdUri { get; set; }
        
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        //public string Issuer { get; set; }

        public string UserAgent { get; set; }
    }

    public class Credential
    {
        public string Name { get; init; }

        public string Key { get; init; }

        public int Value { get; init; }
    }

    public class ClaimConfig
    {
        public string RoleClaimType { get; init; } = "roles"; // ClaimTypes.Role // securityConfig.Claim.RoleClaimType

        //public string NameClaimType { get; init; }

        //public string RoleIT { get; init; }
        public Role[] Roles { get; init; }

        public class Role
        {
            public string Id { get; init; }

            public string Name { get; init; }
            public string[] Credentials { get; init; }
        }

        public string ResourceDisplayNamePrefix { get; init; } 

        public PolicyItem[] Policies { get; init; }

        public class PolicyItem
        {
            public string Name { get; init; }

            public string[] Roles { get; init; }
        }
    }
}