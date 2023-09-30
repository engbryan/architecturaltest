//using Bryan.Proof.Common.Constants;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;

//namespace Bryan.TokenAuth;

//public static class SecurityConfiguration
//{
//    /// <summary>
//    /// Adiciona configuracoes de seguranca no contexto.
//    /// </summary>
//    /// <param name="services"></param>
//    ///
//    private static readonly IDictionary<string, string> _azureConfigLocal = new Dictionary<string, string>()
//    {
//        { SecurityConstants.CLIENT_ID_KEY, "d1b97c53-f513-480d-ba5e-7f9782b6f260" },
//        { SecurityConstants.TENANT_ID_KEY, "16e7cf3f-6af4-4e76-941e-aecafb9704e9" },
//        { SecurityConstants.INSTANCE_KEY, "https://login.microsoftonline.com/" },
//        { SecurityConstants.CLIENT_SECRET_KEY, "bHZ8Q~sRwii06OV8W~Hl5AMqrxCzfimiHqwrWdB1" },
//        { SecurityConstants.APPLICATION_ID_URI_KEY, "api://d1b97c53-f513-480d-ba5e-7f9782b6f260/.default" },
//        { SecurityConstants.USER_AGENT_KEY, "YnRnLW1vdG9yY2FtYmlvLWRldg==" },
//    };

//    public static void AddAzureConfigLocal(this IConfigurationBuilder config) => config.AddInMemoryCollection(_azureConfigLocal);

//    public static IWebHostBuilder AddAzureConfigLambda(IWebHostBuilder builder)
//    {
//        var azureConfigLambda = new Dictionary<string, string>()
//        {
//            { SecurityConstants.CLIENT_ID_KEY, Environment.GetEnvironmentVariable("AzureADClientId")! },
//            { SecurityConstants.TENANT_ID_KEY, Environment.GetEnvironmentVariable("AzureADTenantId")! },
//            { SecurityConstants.INSTANCE_KEY, "https://login.microsoftonline.com/"! },
//            { SecurityConstants.CLIENT_SECRET_KEY, Environment.GetEnvironmentVariable("AzureADClientSecret")! },
//            { SecurityConstants.APPLICATION_ID_URI_KEY, Environment.GetEnvironmentVariable("AzureADApplicationUri")! },
//            { SecurityConstants.USER_AGENT_KEY, Environment.GetEnvironmentVariable("AzureADUserAgent")! },
//        };

//        if (azureConfigLambda.Values.All(v => v != null))
//        {
//            return builder.ConfigureAppConfiguration((hostingContext, config) => config.AddInMemoryCollection(azureConfigLambda));
//        }
//        else
//        {
//            throw new Exception();
//        }
//    }
//}