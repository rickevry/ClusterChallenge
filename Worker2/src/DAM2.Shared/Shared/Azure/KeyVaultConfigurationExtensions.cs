using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DAM2.Core.Shared.Azure
{
    public static class KeyVaultConfigurationExtensions
    {
	    public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder, KeyVaultOptions keyVaultOptions, PrefixKeyVaultSecretManagerOptions secretManagerOptions)
	    {
		    string keyVaultUri = keyVaultOptions.BaseUri;
		    Log.Logger.Information("Connecting to key vault {keyvault}", keyVaultUri);

		    var keyVaultClient = GetKeyVaultClient();

		    Log.Logger.Information("AddAzureKeyVault");
		    builder.AddAzureKeyVault(
			    keyVaultUri,
			    keyVaultClient,
			    new PrefixKeyVaultSecretManager(secretManagerOptions));

		    return builder;
	    }

	    public static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder builder, KeyVaultOptions keyVaultOptions)
	    {
		    string keyVaultUri = keyVaultOptions.BaseUri;
		    Log.Logger.Information("Connecting to key vault {keyvault}", keyVaultUri);

		    var keyVaultClient = GetKeyVaultClient();

		    Log.Logger.Information("AddAzureKeyVault");
		    builder.AddAzureKeyVault(
			    keyVaultUri,
			    keyVaultClient,
			    new DefaultKeyVaultSecretManager());

		    return builder;
	    }

	    public static IServiceCollection AddKeyVaultOptions(this IServiceCollection services, IConfiguration configuration, string key = "KeyVault")
	    {
		    services.Configure<KeyVaultOptions>(o => { configuration.Bind(key, o); });
		    return services;
	    }

	    private static KeyVaultClient GetKeyVaultClient()
	    {


		    var azureServiceTokenProvider = new AzureServiceTokenProvider();
		    var keyVaultClient = new KeyVaultClient(
			    new KeyVaultClient.AuthenticationCallback(
				    azureServiceTokenProvider.KeyVaultTokenCallback));
		    return keyVaultClient;
	    }
    }
}
