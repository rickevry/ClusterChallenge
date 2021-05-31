using System;
using System.Linq;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace DAM2.Core.Shared.Azure
{
	public class PrefixKeyVaultSecretManagerOptions
	{
		public string Prefix { get; set; }
		public string[] Prefixes { get; set; } = { };
	}
	public class PrefixKeyVaultSecretManager : IKeyVaultSecretManager
    {
	    private readonly PrefixKeyVaultSecretManagerOptions options;

	    public PrefixKeyVaultSecretManager(PrefixKeyVaultSecretManagerOptions options)
	    {
		    this.options = options;
	    }
	    public bool Load(SecretItem secret)
	    {
			if (!string.IsNullOrWhiteSpace(this.options.Prefix))
			{
				return secret.Identifier.Name.StartsWith(this.options.Prefix, StringComparison.OrdinalIgnoreCase);
			}

			if (!this.options.Prefixes.Any())
			{
				return true;
			}

			return this.options.Prefixes.Any(prefix => secret.Identifier.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
		}

	    public string GetKey(SecretBundle secret)
	    {
			if (!string.IsNullOrWhiteSpace(this.options.Prefix))
			{
				return secret.SecretIdentifier.Name
					.Substring(this.options.Prefix.Length)
					.Replace("--", ConfigurationPath.KeyDelimiter);
			}

			if (!this.options.Prefixes.Any())
			{
				return secret.SecretIdentifier.Name;
			}

			foreach (var prefix in this.options.Prefixes)
			{
				if (secret.SecretIdentifier.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					return secret.SecretIdentifier.Name
						.Substring(prefix.Length)
						.Replace("--", ConfigurationPath.KeyDelimiter);
				}
			}

			throw new Exception("Secret key not found");
		}
    }
}
