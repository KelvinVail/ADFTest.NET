using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace ADFTests.Net.IntegrationTests.Helpers
{
    public class SettingsHelper
    {
        private readonly SecretClient _keyVaultClient;

        private readonly IConfigurationRoot _config;

        public SettingsHelper()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", false)
                .Build();

            var kvUrl = _config.GetSection("KeyVaultUrl").Value;
            var tenantId = _config.GetSection("AZURE_TENANT_ID").Value;
            var clientId = _config.GetSection("AZURE_CLIENT_ID").Value;
            var clientSecret = _config.GetSection("AZURE_CLIENT_SECRET").Value;
            if (kvUrl?.Length > 0)
                _keyVaultClient = new SecretClient(new Uri(kvUrl), new ClientSecretCredential(tenantId, clientId, clientSecret));
        }

        public string GetSetting(string settingName)
        {
            // return environment variable "settingName", if present
            var value = Environment.GetEnvironmentVariable(settingName);
            if (value?.Length > 0)
                return value;

            // return value of runsettings parameter "settingName", if present
            value = _config.GetSection(settingName).Value;
            if (value?.Length > 0)
                return value;

            // if a key vault is specified, return the value of secret "settingName", if present
            if (_keyVaultClient != null)
            {
                value = _keyVaultClient.GetSecret(settingName).Value.Value;
                if (value?.Length > 0)
                    return value;
            }

            throw new Exception($"Test setting '{settingName}' not found");
        }
    }
}
