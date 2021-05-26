using System;
using System.Collections.Generic;
using ADFTest.Net.Application.Contracts;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace ADFTest.Net.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRoot _config;
        private readonly SecretClient _keyVault;

        public ConfigurationService(IConfigurationRoot config, SecretClient keyVault)
        {
            _config = config;
            _keyVault = keyVault;
        }

        public string GetConfiguration(string name)
        {
            if (name is null) return null;

            var value = CheckEnvironment(name);
            if (value != null) return value;

            value = CheckConfiguration(name);
            if (value != null) return value;

            value = CheckKeyVault(name);
            if (value != null) return value;

            throw new KeyNotFoundException($"The configuration '{name}' was not found.");
        }

        private static string CheckEnvironment(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }

        private string CheckConfiguration(string name)
        {
            return _config.GetSection(name).Value;
        }

        private string CheckKeyVault(string name)
        {
            return _keyVault.GetSecret(name)?.Value?.Value;
        }
    }
}
