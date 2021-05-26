using System.Collections.Generic;
using System.Threading;
using Azure;
using Azure.Security.KeyVault.Secrets;

namespace ADFTest.Net.Services.Tests.TestDoubles
{
    public class SecretClientStub : SecretClient
    {
        private readonly Dictionary<string, KeyVaultSecret> _secrets = new ();

        public bool Called { get; set; }

        public override Response<KeyVaultSecret> SetSecret(
            string name,
            string value,
            CancellationToken cancellationToken = default)
        {
            _secrets.Add(name, new KeyVaultSecret(name, value));

            return default;
        }

        public override Response<KeyVaultSecret> GetSecret(
            string name,
            string version = null,
            CancellationToken cancellationToken = default)
        {
            Called = true;

            if (!_secrets.ContainsKey(name)) return default;

            return Response.FromValue(_secrets[name], null!);
        }
    }
}
