using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ADFTest.Net.Services.Tests.TestDoubles
{
    public class ConfigurationRootStub : IConfigurationRoot
    {
        private readonly Dictionary<string, string> _values = new ();

        public IConfigurationSection GetSection(string key)
        {
            Called = true;

            return new ConfigurationSectionStub
            {
                Key = key,
                Value = _values.ContainsKey(key) ? _values[key] : null,
            };
        }

        public IEnumerable<IConfigurationProvider> Providers { get; }

        public bool Called { get; set; }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new System.NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new System.NotImplementedException();
        }

        public string this[string key]
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public void Reload()
        {
            throw new System.NotImplementedException();
        }

        public void Add(string key, string value)
        {
            _values.Add(key, value);
        }
    }
}
